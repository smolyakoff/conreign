using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using Microsoft.AspNet.SignalR;

namespace Conreign.Api.Hubs
{
    public class GameHub : Hub<IObserver<MessageEnvelope>>
    {
        private static readonly ConcurrentDictionary<string, GameHubConnection> Handlers = new ConcurrentDictionary<string, GameHubConnection>();
        private readonly OrleansGameClient _client;

        public GameHub(OrleansGameClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            _client = client;
        }

        public async Task<MessageEnvelope> Send(MessageEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }
            var handler = GetHandlerSafely();
            var result = await handler.Handle((dynamic)envelope.Payload, envelope.Meta);
            return new MessageEnvelope {Payload = result};
        }

        public Task Post(MessageEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }
            var handler = GetHandlerSafely();
            return handler.Handle((dynamic)envelope.Payload, envelope.Meta);
        }

        public override async Task OnConnected()
        {
            var connection = await _client.Connect(Guid.Parse(Context.ConnectionId));
            var handler = new GameHandler(connection);
            var subscription = handler.Events.Subscribe(new ConnectionObserver(this, Context.ConnectionId));
            Handlers[Context.ConnectionId] = new GameHubConnection(handler, subscription);
            await base.OnConnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GameHubConnection hubConnection;
            var removed = Handlers.TryRemove(Context.ConnectionId, out hubConnection);
            if (!removed)
            {
                return Task.CompletedTask;
            }
            hubConnection.Subscription.Dispose();
            hubConnection.Handler.Dispose();
            return base.OnDisconnected(stopCalled);
        }

        private IGameHandler GetHandlerSafely()
        {
            GameHubConnection connection;
            var exists = Handlers.TryGetValue(Context.ConnectionId, out connection);
            if (!exists)
            {
                throw new InvalidOperationException($"Handler was not found for connection id: {Context.ConnectionId}.");
            }
            return connection.Handler;
        }

        private class ConnectionObserver : IObserver<IClientEvent>
        {
            private readonly GameHub _hub;
            private readonly string _connectionId;

            public ConnectionObserver(GameHub hub, string connectionId)
            {
                _hub = hub;
                _connectionId = connectionId;
            }

            public void OnNext(IClientEvent value)
            {
                _hub.Clients.Client(_connectionId).OnNext(new MessageEnvelope { Payload = value });
            }

            public void OnError(Exception error)
            {
                _hub.Clients.Client(_connectionId).OnError(error);
            }

            public void OnCompleted()
            {
                _hub.Clients.Client(_connectionId).OnCompleted();
            }
        }

        private class GameHubConnection
        {
            public GameHubConnection(GameHandler handler, IDisposable subscription)
            {
                Handler = handler;
                Subscription = subscription;
            }

            public GameHandler Handler { get; }
            public IDisposable Subscription { get; }
        }
    }
}
