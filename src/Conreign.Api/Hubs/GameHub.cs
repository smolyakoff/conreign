using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Conreign.Client.Handler;
using Conreign.Client.Orleans;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using MediatR;
using Microsoft.AspNet.SignalR;

namespace Conreign.Api.Hubs
{
    public class GameHub : Hub<IObserver<MessageEnvelope>>
    {
        private static readonly ConcurrentDictionary<string, GameHubConnection> Handlers =
            new ConcurrentDictionary<string, GameHubConnection>();

        private readonly OrleansClient _client;
        private readonly IMediator _mediator;

        public GameHub(OrleansClient client, IMediator mediator)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            _client = client;
            _mediator = mediator;
        }

        public async Task<MessageEnvelope> Send(MessageEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }
            var handler = GetHandlerSafely();
            var result = await handler.Handle((dynamic) envelope.Payload, envelope.Meta);
            return new MessageEnvelope {Payload = result};
        }

        public Task Post(MessageEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }
            var handler = GetHandlerSafely();
            return handler.Handle((dynamic) envelope.Payload, envelope.Meta);
        }

        public override async Task OnConnected()
        {
            var connection = await _client.Connect(Guid.Parse(Context.ConnectionId));
            var handler = new ClientHandler(connection, _mediator);
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

        private IClientHandler GetHandlerSafely()
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
            private readonly string _connectionId;
            private readonly GameHub _hub;

            public ConnectionObserver(GameHub hub, string connectionId)
            {
                _hub = hub;
                _connectionId = connectionId;
            }

            public void OnNext(IClientEvent value)
            {
                _hub.Clients.Client(_connectionId).OnNext(new MessageEnvelope {Payload = value});
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
            public GameHubConnection(ClientHandler handler, IDisposable subscription)
            {
                Handler = handler;
                Subscription = subscription;
            }

            public ClientHandler Handler { get; }
            public IDisposable Subscription { get; }
        }
    }
}