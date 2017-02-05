using System;
using System.Reactive.Subjects;
using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Exceptions;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Streams;

namespace Conreign.Client.Orleans
{
    public class OrleansClientConnection : IClientConnection
    {
        private readonly IGrainFactory _factory;
        private readonly Subject<IClientEvent> _subject;
        private bool _isDisposed;
        private StreamSubscriptionHandle<IClientEvent> _stream;

        private OrleansClientConnection(Guid connectionId, IGrainFactory factory)
        {
            Id = connectionId;
            _subject = new Subject<IClientEvent>();
            _factory = factory;
        }

        public Guid Id { get; }

        public IObservable<IClientEvent> Events => _subject;

        public async Task<LoginResult> Login(string accessToken = null)
        {
            EnsureIsNotDisposed();
            var auth = _factory.GetGrain<IAuthGrain>(default(long));
            var token = await auth.Login(accessToken);
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

        internal static async Task<OrleansClientConnection> Initialize(IGrainFactory grainFactory, Guid connectionId)
        {
            var stream = GrainClient.GetStreamProvider(StreamConstants.ProviderName)
                .GetStream<IClientEvent>(connectionId, StreamConstants.ClientNamespace);
            var existingHandles = await stream.GetAllSubscriptionHandles();
            var connection = new OrleansClientConnection(connectionId, grainFactory);
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

        private Task OnNext(IClientEvent @event, StreamSequenceToken token)
        {
            _subject.OnNext(@event);
            return TaskCompleted.Completed;
        }

        private Task OnError(Exception exception)
        {
            _subject.OnError(exception);
            return TaskCompleted.Completed;
        }

        private async Task OnCompleted()
        {
            if (!_isDisposed)
            {
                _subject.OnError(new ConnectionException("Connection was forcibly closed by the server."));
                _isDisposed = true;
            }
            await _stream.UnsubscribeAsync();
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
            var universe = _factory.GetGrain<IConnectionGrain>(Id);
            universe.Disconnect();
        }
    }
}