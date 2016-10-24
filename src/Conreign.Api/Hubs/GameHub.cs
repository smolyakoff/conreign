using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Client;
using Conreign.Core.Client.Messages;
using Conreign.Core.Contracts.Communication;
using MediatR;
using Microsoft.AspNet.SignalR;

namespace Conreign.Api.Hubs
{
    public class GameHub : Hub<IObserver<IClientEvent>>
    {
        private readonly ConcurrentDictionary<string, GameHubConnection> _handlers;
        private readonly GameClient _client;

        public GameHub(GameClient client)
        {
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            _client = client;
            _handlers = new ConcurrentDictionary<string, GameHubConnection>();
        }

        public Task<object> Send(IAsyncRequest<object> command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var handler = GetHandlerSafely();
            var meta = ParseMetadata();
            return handler.Handle(command, meta);
        }

        public Task Post(IAsyncRequest command)
        {
            if (command == null)
            {
                throw new ArgumentNullException(nameof(command));
            }
            var handler = GetHandlerSafely();
            var meta = ParseMetadata();
            return handler.Handle(command, meta);
        }

        public override async Task OnConnected()
        {
            var connection = await _client.Connect(Guid.Parse(Context.ConnectionId));
            var handler = new GameHandler(connection);
            var subscription = handler.Events.Subscribe(new ConnectionObserver(this, Context.ConnectionId));
            _handlers[Context.ConnectionId] = new GameHubConnection(handler, subscription);
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            GameHubConnection hubConnection;
            var removed = _handlers.TryRemove(Context.ConnectionId, out hubConnection);
            if (!removed)
            {
                return Task.CompletedTask;
            }
            hubConnection.Subscription.Dispose();
            hubConnection.Handler.Dispose();
            return base.OnDisconnected(stopCalled);
        }

        private Metadata ParseMetadata()
        {
            return new Metadata
            {
                AccessToken = ParseAccessToken()
            };
        }

        private string ParseAccessToken()
        {
            var authorizationHeader = Context.Headers.Get("Authorization");
            var accessToken = authorizationHeader?.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase) == true
                ? authorizationHeader.Split(' ').Skip(1).First()
                : null;
            return accessToken;
        }

        private IGameHandler GetHandlerSafely()
        {
            GameHubConnection connection;
            var exists = _handlers.TryGetValue(Context.ConnectionId, out connection);
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
                _hub.Clients.Client(_connectionId).OnNext(value);
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
