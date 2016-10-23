using System;
using System.Reactive.Subjects;
using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Core.Client.Exceptions;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Runtime;
using Orleans.Streams;

namespace Conreign.Core.Client
{
    public class OrleansGameConnection : IDisposable, IGameConnection
    {
        private readonly IGrainFactory _factory;
        private StreamSubscriptionHandle<IClientEvent> _stream;
        private bool _isDisposed;
        private readonly ISubject<IClientEvent> _subject;

        internal static async Task<OrleansGameConnection> Initialize(IGrainFactory grainFactory, Guid connectionId)
        {
            var universe = grainFactory.GetGrain<IUniverseGrain>(default(long));
            await universe.Ping();
            var stream = GrainClient.GetStreamProvider(StreamConstants.ProviderName)
                .GetStream<IClientEvent>(connectionId, StreamConstants.ClientNamespace);
            var existingHandles = await stream.GetAllSubscriptionHandles();
            var connection = new OrleansGameConnection(connectionId, grainFactory);
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

        private OrleansGameConnection(Guid connectionId, IGrainFactory factory)
        {
            Id = connectionId;
            _subject = new Subject<IClientEvent>();
            _factory = factory;
        }

        public Guid Id { get; }

        public IObservable<IClientEvent> Events => _subject;

        public async Task<LoginResult> Login()
        {
            EnsureIsNotDisposed();
            var auth = _factory.GetGrain<IAuthGrain>(default(long));
            var token = await auth.Login();
            return await Authenticate(token);
        }

        public async Task<LoginResult> Authenticate(string accessToken)
        {
            if (string.IsNullOrEmpty(accessToken))
            {
                throw new ArgumentException("Access token cannot be null or empty.", nameof(accessToken));
            }
            EnsureIsNotDisposed();
            var auth = _factory.GetGrain<IAuthGrain>(default(long));
            var identity = await auth.Authenticate(accessToken);
            var userId = Guid.Parse(identity.FindFirst(x => x.Type == ClaimTypes.NameIdentifier).Value);
            var user = _factory.GetGrain<IUserGrain>(userId);
            return new LoginResult(user, userId, accessToken);
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