using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Server.Gameplay;
using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Conreign.Server.Contracts.Shared.Gameplay.Events;
using Conreign.Server.Core.Communication;
using Orleans;
using Orleans.Streams;
using Serilog;

namespace Conreign.Server.Core.Gameplay;

public class LobbyGrain : Grain<LobbyState>, ILobbyGrain
{
    private readonly LobbyGrainOptions _options;
    private IDisposable _inactivityTimer;
    private Lobby _lobby;
    private ILogger _logger;
    private StreamSubscriptionHandle<IServerEvent> _subscription;

    public LobbyGrain(ILogger logger, LobbyGrainOptions options)
    {
        _logger = logger.ForContext(GetType()) ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
    }

    public Task<IRoomData> GetState(Guid userId)
    {
        return _lobby.GetState(userId);
    }

    public Task Notify(ISet<Guid> userIds, params IEvent[] @event)
    {
        return _lobby.Notify(userIds, @event);
    }

    public Task NotifyEverybody(params IEvent[] @event)
    {
        return _lobby.NotifyEverybody(@event);
    }

    public Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events)
    {
        return _lobby.NotifyEverybodyExcept(userIds, events);
    }

    public Task Connect(Guid userId, string connectionId)
    {
        return _lobby.Connect(userId, connectionId);
    }

    public Task Disconnect(Guid userId, string connectionId)
    {
        return _lobby.Disconnect(userId, connectionId);
    }

    public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
    {
        await _lobby.UpdateGameOptions(userId, options);
        await WriteStateAsync();
    }

    public async Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
    {
        await _lobby.UpdatePlayerOptions(userId, options);
        await WriteStateAsync();
    }

    public async Task<IGame> StartGame(Guid userId)
    {
        var game = await _lobby.StartGame(userId);
        return game;
    }

    public async Task<IGame> CreateGame(Guid userId)
    {
        var game = GrainFactory.GetGrain<IGameGrain>(this.GetPrimaryKeyString());
        var command = new InitialGameData(
            userId,
            State.MapEditor.Map,
            State.Players,
            State.Hub.Members.ToDictionary(x => x.Key, x => x.Value.ConnectionIds),
            State.Hub.JoinOrder
        );
        await game.Initialize(command);
        return game;
    }

    public async Task Handle(GameEnded @event)
    {
        await _lobby.Handle(@event);
        DeactivateOnIdle();
    }

    public override async Task OnActivateAsync()
    {
        InitializeState();
        var topic = Topic.Room(GetStreamProvider(StreamConstants.ProviderName), this.GetPrimaryKeyString());
        _logger = _logger.ForContext(nameof(State.RoomId), State.RoomId);
        _logger.Information("Lobby is activated.", State.RoomId);
        _lobby = new Lobby(State, topic, this);
        _subscription = await topic.EnsureIsSubscribedOnce(this);
        var inactivityTimerInterval = TimeSpan.FromTicks(_options.MaxInactivityPeriod.Ticks / 2);
        _inactivityTimer = RegisterTimer(
            EnsureSomeoneIsOnline,
            null,
            inactivityTimerInterval,
            inactivityTimerInterval);
        await base.OnActivateAsync();
    }

    public override async Task OnDeactivateAsync()
    {
        await _subscription.UnsubscribeAsync();
        _inactivityTimer?.Dispose();
        await ClearStateAsync();
        await base.OnDeactivateAsync();
    }

    private Task EnsureSomeoneIsOnline(object state)
    {
        if (_lobby.EveryoneOfflinePeriod > _options.MaxInactivityPeriod)
        {
            _logger.Information(
                "Going to deactivate lobby due to inactivity. Inactivity period was {InactivityPeriod}.",
                _lobby.EveryoneOfflinePeriod);
            _inactivityTimer?.Dispose();
            _inactivityTimer = null;
            DeactivateOnIdle();
        }

        return Task.CompletedTask;
    }

    private void InitializeState()
    {
        State.RoomId = this.GetPrimaryKeyString();
        State.Hub.Id = State.RoomId;
    }
}