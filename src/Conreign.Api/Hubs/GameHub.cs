using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Conreign.Client.Handler;
using Conreign.Client.Orleans;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Exceptions;
using Conreign.Core.Contracts.Communication;
using MediatR;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using SerilogMetrics;

namespace Conreign.Api.Hubs
{
    public class GameHub : Hub<IObserver<MessageEnvelope>>
    {
        private static readonly ConcurrentDictionary<string, GameHubConnection> Handlers =
            new ConcurrentDictionary<string, GameHubConnection>();

        private readonly OrleansClient _client;
        private readonly GameHubCountersCollection _countersCollection;
        private readonly IMediator _mediator;
        private readonly JsonSerializer _errorSerializer;
        private readonly ILogger _logger;
        private readonly IGaugeMeasure _connectionsGauge;

        public GameHub(ILogger logger, OrleansClient client, GameHubCountersCollection countersCollection, IMediator mediator)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            if (client == null)
            {
                throw new ArgumentNullException(nameof(client));
            }
            if (countersCollection == null)
            {
                throw new ArgumentNullException(nameof(countersCollection));
            }
            if (mediator == null)
            {
                throw new ArgumentNullException(nameof(mediator));
            }
            _logger = logger;
            _client = client;
            _countersCollection = countersCollection;
            _mediator = mediator;
            _errorSerializer = JsonSerializer.Create(new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.All
            });
            _connectionsGauge = _logger.GaugeOperation("Hub.Connections", "connection(s)", () => Handlers.Count);
        }


        public async Task<MessageEnvelope> Send(MessageEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }
            var result = await Handle((dynamic)envelope.Payload, envelope.Meta);
            return new MessageEnvelope { Payload = result };
        }

        public Task Post(MessageEnvelope envelope)
        {
            if (envelope == null)
            {
                throw new ArgumentNullException(nameof(envelope));
            }
            return Handle((dynamic)envelope.Payload, envelope.Meta);
        }

        private async Task<T> Handle<T>(IRequest<T> command, Metadata metadata)
        {
            var handler = GetHandlerSafely();
            try
            {
                var result = await handler.Handle(command, metadata);
                return result;
            }
            catch (UserException ex)
            {
                var error = ex.ToUserError();
                var jError = JObject.FromObject(error, _errorSerializer);
                jError["$nettype"] = jError["$type"];
                throw new HubException(error.Message, jError);
            }
        }

        public override async Task OnConnected()
        {
            using (LogContext.PushProperty("ConnectionId", Context.ConnectionId))
            {
                var connection = await _client.Connect(Guid.Parse(Context.ConnectionId));
                var handler = new ClientHandler(connection, _mediator);
                var subscription = handler.Events.Subscribe(new ConnectionObserver(this, Context.ConnectionId));
                Handlers[Context.ConnectionId] = new GameHubConnection(handler, subscription);
                _connectionsGauge.Write();
                await base.OnConnected();
            }

        }

        public override Task OnDisconnected(bool stopCalled)
        {
            var props = new ILogEventEnricher[]
            {
                new PropertyEnricher("ConnectionId", Context.ConnectionId),
                new PropertyEnricher("StopCalled", stopCalled),
            };
            using (LogContext.PushProperties(props))
            {
                GameHubConnection hubConnection;
                var removed = Handlers.TryRemove(Context.ConnectionId, out hubConnection);
                if (!removed)
                {
                    return TaskCompleted.Completed;
                }
                _connectionsGauge.Write();
                hubConnection.Subscription.Dispose();
                hubConnection.Handler.Dispose();
                return base.OnDisconnected(stopCalled);
            }
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
                using (LogContext.PushProperty("ConnectionId", _connectionId))
                {
                    _hub._countersCollection.EventsReceived.Increment();
                    _hub.Clients.Client(_connectionId).OnNext(new MessageEnvelope { Payload = value });
                    _hub._countersCollection.EventsDispatched.Increment();
                }
            }

            public void OnError(Exception exception)
            {
                using (LogContext.PushProperty("ConnectionId", _connectionId))
                {
                    _hub._logger.Error(exception, "Stream exception received: {Message}", exception.Message);
                    _hub._countersCollection.ErrorsReceived.Increment();
                    _hub.Clients.Client(_connectionId).OnError(exception);
                    _hub._countersCollection.ErrorsDispatched.Increment();
                }
            }

            public void OnCompleted()
            {
                using (LogContext.PushProperty("ConnectionId", _connectionId))
                {
                    _hub.Clients.Client(_connectionId).OnCompleted();
                    _hub._countersCollection.StreamsCompleted.Increment();
                }
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