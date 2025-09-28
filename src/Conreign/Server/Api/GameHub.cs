using System.Collections.Concurrent;
using Conreign.Server.Api.Handler;
using Conreign.Server.Contracts.Client;
using Conreign.Server.Contracts.Shared.Errors;
using MediatR;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using Orleans;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using SerilogMetrics;

namespace Conreign.Server.Api;

public class GameHub : Hub<IGameHubClient>
{
    private static readonly ConcurrentDictionary<string, GameHubConnection> Connections = new();

    private readonly IClusterClient _clusterClient;

    private readonly IGaugeMeasure _connectionsGauge;
    private readonly GameHubCountersCollection _countersCollection;
    private readonly JsonSerializer _errorSerializer;
    private readonly IHubContext<GameHub, IGameHubClient> _hubContext;
    private readonly ILogger _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public GameHub(
        ILogger logger,
        IClusterClient clusterClient,
        IServiceScopeFactory serviceScopeFactory,
        IHubContext<GameHub, IGameHubClient> hubContext,
        GameHubCountersCollection countersCollection)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _clusterClient = clusterClient ?? throw new ArgumentNullException(nameof(clusterClient));
        _serviceScopeFactory = serviceScopeFactory ?? throw new ArgumentNullException(nameof(serviceScopeFactory));
        _hubContext = hubContext ?? throw new ArgumentNullException(nameof(hubContext));
        _countersCollection = countersCollection ?? throw new ArgumentNullException(nameof(countersCollection));
        _errorSerializer = JsonSerializer.Create(new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Objects,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        });
        _connectionsGauge = _logger.GaugeOperation("Hub.Connections", "connection(s)", () => Connections.Count);
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

    public override async Task OnConnectedAsync()
    {
        using (LogContext.PushProperty("ConnectionId", Context.ConnectionId))
        {
            var connection =
                await OrleansClientConnection.Initialize(_clusterClient, _hubContext, Context.ConnectionId);
            Connections[Context.ConnectionId] = new GameHubConnection(
                connection,
                _serviceScopeFactory);
            _connectionsGauge.Write();
            await base.OnConnectedAsync();
        }
    }

    public override async Task OnDisconnectedAsync(Exception exception)
    {
        var props = new ILogEventEnricher[]
        {
            new PropertyEnricher("ConnectionId", Context.ConnectionId)
        };
        using (LogContext.Push(props))
        {
            var removed = Connections.TryRemove(Context.ConnectionId, out var hubConnection);
            if (!removed)
            {
                return;
            }

            _connectionsGauge.Write();
            hubConnection.Dispose();
            await base.OnDisconnectedAsync(exception);
        }
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
            var hubException = new HubException(error.Message);
            hubException.Data["error"] = error;
            // TODO: Previously was different code. Figure out how to pass errors to the client.
            // throw new HubException(error.Message, jError);
            throw hubException;
        }
    }

    private IClientHandler GetHandlerSafely()
    {
        var exists = Connections.TryGetValue(Context.ConnectionId, out var connection);
        if (!exists)
        {
            throw new InvalidOperationException(
                $"Handler was not found for connection id: {Context.ConnectionId}.");
        }

        return connection.Handler;
    }

    private class GameHubConnection : IDisposable
    {
        private readonly IClientConnection _connection;

        public GameHubConnection(
            IClientConnection connection,
            IServiceScopeFactory serviceScopeFactory)
        {
            var handler = new ClientHandler(connection, serviceScopeFactory);
            Handler = handler;
            _connection = connection;
        }

        public IClientHandler Handler { get; }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}