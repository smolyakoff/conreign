using System;
using System.IO;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Conreign.Core.Client.Commands;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;
using Polly;

namespace Conreign.Core.Client
{
    public class MessageMetadata
    {
        public string AccessToken { get; set; }
        public Guid? UserId { get; set; }
        public string TraceId { get; set; }
    }

    public class Message<T>
    {
        public MessageMetadata Meta { get; set; }
        public T Payload { get; set; }
    }

    public class GameConnection : IDisposable
    {
        private readonly IGrainFactory _factory;
        private StreamSubscriptionHandle<object> _stream;
        private bool _disposed;
        private ISubject<object> _subject;

        internal static async Task<GameConnection> Initialize(IGrainFactory grainFactory, Guid connectionId)
        {
            var stream = GrainClient.GetStreamProvider(StreamConstants.ClientStreamProviderName)
                .GetStream<object>(connectionId, StreamConstants.ClientStreamNamespace);
            var existingHandles = await stream.GetAllSubscriptionHandles();
            var connection = new GameConnection(connectionId, grainFactory);
            var handle = existingHandles.Count > 0 
                ? existingHandles[0] 
                : await stream.SubscribeAsync(connection.OnNext, connection.OnError, connection.OnCompleted);
            if (existingHandles.Count > 0)
            {
                await handle.ResumeAsync(connection.OnNext, connection.OnError, connection.OnCompleted);
            }
            connection._stream = handle;
            return connection;
        }

        private GameConnection(Guid connectionId, IGrainFactory factory)
        {
            Id = connectionId;
            _subject = new Subject<object>();
            _factory = factory;
        }

        public Guid Id { get; }

        public IUser Login()
        {
            return Authenticate(null);
        }

        public IUser Authenticate(string accessToken)
        {
            var userId = Guid.NewGuid();
            PrepareContext(userId);
            return _factory.GetGrain<IUserGrain>(userId);
        }

        public IObservable<object> Events => _subject;

        private void PrepareContext(Guid? userId)
        {
            RequestContext.Set("ConnectionId", Id);
            RequestContext.Set("UserId", userId);
        }

        private Task OnNext(object @event, StreamSequenceToken token)
        {
            _subject.OnNext(@event);
            return Task.CompletedTask;
        }

        private Task OnError(Exception exception)
        {
            _subject.OnError(exception);
            return Task.CompletedTask;
        }

        private Task OnCompleted()
        {
            _subject.OnCompleted();
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _stream.UnsubscribeAsync();
        }
    }

    public class GameClient
    {
        private readonly IGrainFactory _factory;

        private GameClient(IGrainFactory factory)
        {
            _factory = factory;
        }

        public static Task<GameClient> Initialize(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new ArgumentException("Config file path cannot be null or empty.", nameof(configFilePath));
            }
            if (!File.Exists(configFilePath))
            {
                throw new ArgumentException($"Orleans client config not found at: {Path.GetFullPath(configFilePath)}.");
            }
            if (!GrainClient.IsInitialized)
            {
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(attempt * 3));
                policy.Execute(() => GrainClient.Initialize(configFilePath));
            }
            var client = new GameClient(GrainClient.GrainFactory);
            return Task.FromResult(client);
        }

        public Task<GameConnection> Connect(Guid connectionId)
        {
            return GameConnection.Initialize(_factory, connectionId);
        }
    }
}
