using System.Security.Claims;
using Conreign.Server.Contracts.Client;
using Conreign.Server.Contracts.Server.Auth;
using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Server.Gameplay;
using Conreign.Server.Contracts.Server.Presence;
using Conreign.Server.Contracts.Shared.Communication;
using Microsoft.AspNetCore.SignalR;
using Orleans;
using Orleans.Streams;

namespace Conreign.Server.Api;

public class OrleansClientConnection : IClientConnection
{
    private readonly IGrainFactory _factory;
    private readonly IHubContext<GameHub, IGameHubClient> _gameHubContext;
    private bool _isDisposed;
    private StreamSubscriptionHandle<IClientEvent> _stream;

    private OrleansClientConnection(string connectionId, IGrainFactory factory,
        IHubContext<GameHub, IGameHubClient> gameHubContext)
    {
        Id = connectionId;
        _factory = factory;
        _gameHubContext = gameHubContext;
    }

    public string Id { get; }

    public async Task<LoginResult> Login(string accessToken = null)
    {
        EnsureIsNotDisposed();
        var auth = _factory.GetGrain<IAuthGrain>(default);
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
        var auth = _factory.GetGrain<IAuthGrain>(default);
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

        // TODO: Async disposal
        // Don't care about the task results here
        Disconnect();
    }

    internal static async Task<OrleansClientConnection> Initialize(IClusterClient clusterClient,
        IHubContext<GameHub, IGameHubClient> gameHubContext, string connectionId)
    {
        var stream = clusterClient.GetStreamProvider(StreamConstants.ProviderName)
            .GetStream<IClientEvent>(Guid.Empty, StreamConstants.GetClientNamespace(connectionId));
        var existingHandles = await stream.GetAllSubscriptionHandles();
        var connection = new OrleansClientConnection(connectionId, clusterClient, gameHubContext);
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
        var message = new MessageEnvelope { Payload = @event };
        return _gameHubContext.Clients.Client(Id).OnNext(message);
    }

    private Task OnError(Exception exception)
    {
        return _gameHubContext.Clients.Client(Id).OnError(exception);
    }

    private async Task OnCompleted()
    {
        var client = _gameHubContext.Clients.Client(Id);
        if (!_isDisposed)
        {
            await client.OnError(new ConnectionException("Connection was forcibly closed by the server."));
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