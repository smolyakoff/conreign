using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using Conreign.Core.Client.Exceptions;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace Conreign.Core.Client
{
    public class GameConnection : IDisposable
    {
        private readonly IGrainFactory _factory;
        private StreamSubscriptionHandle<IClientEvent> _stream;
        private bool _isDisposed;
        private readonly ISubject<IClientEvent> _subject;
        private readonly TimeSpan _eventWaitTimeout = TimeSpan.FromSeconds(10);

        internal static async Task<GameConnection> Initialize(IGrainFactory grainFactory, Guid connectionId)
        {
            var universe = grainFactory.GetGrain<IUniverseGrain>(default(long));
            await universe.Ping();
            var stream = GrainClient.GetStreamProvider(StreamConstants.ClientStreamProviderName)
                .GetStream<IClientEvent>(connectionId, StreamConstants.ClientStreamNamespace);
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
            _subject = new Subject<IClientEvent>();
            _factory = factory;
        }

        public Guid Id { get; }

        public IObservable<object> Events => _subject;

        public IUser Login()
        {
            return Authenticate(null);
        }

        public IUser Authenticate(string accessToken)
        {
            EnsureIsNotDisposed();
            var userId = Guid.NewGuid();
            PrepareContext(userId);
            return _factory.GetGrain<IUserGrain>(userId);
        }

        public async Task<T> WaitFor<T>() where T : IClientEvent
        {
            var tasks = new[]
            {
                Events.OfType<T>().FirstAsync().ToTask(),
                Task.Run(new Func<Task<T>>(async () =>
                {
                    await Task.Delay(_eventWaitTimeout);
                    throw new TimeoutException($"Timeouted waiting for event: {typeof(T).Name}");
                })),
            };
            return await await Task.WhenAny(tasks);
        }

        private void PrepareContext(Guid? userId)
        {
            RequestContext.Set("ConnectionId", Id);
            RequestContext.Set("UserId", userId);
        }

        private Task OnNext(IClientEvent @event, StreamSequenceToken token)
        {
            _subject.OnNext(@event);
            return Task.CompletedTask;
        }

        private Task OnError(Exception exception)
        {
            _subject.OnError(exception);
            return Task.CompletedTask;
        }

        private async Task OnCompleted()
        {
            if (!_isDisposed)
            {
                _subject.OnError(new ConnectionException("Connection was forcibly closed by the server."));
                _isDisposed = true;
            }
            await _stream.UnsubscribeAsync();
            _subject.OnCompleted();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            _isDisposed = true;
            if (!disposing)
            {
                return;
            }
            // Don't care about the task results here
            Disconnect();
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException($"GameConnection: {Id}");
            }
        }

        private void Disconnect()
        {
            var universe = _factory.GetGrain<IUniverseGrain>(default(long));
            universe.Disconnect(Id);
        }
    }
}