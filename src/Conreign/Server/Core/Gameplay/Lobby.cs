﻿using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Server.Gameplay;
using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Errors;
using Conreign.Server.Contracts.Shared.Gameplay;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Conreign.Server.Contracts.Shared.Gameplay.Events;
using Conreign.Server.Contracts.Shared.Presence;
using Conreign.Server.Core.Communication;
using Conreign.Server.Core.Editor;
using Conreign.Server.Core.Presence;

namespace Conreign.Server.Core.Gameplay;

public class Lobby : ILobby, IEventHandler<GameEnded>
{
    private readonly IGameFactory _gameFactory;
    private readonly LobbyState _state;
    private readonly IUserTopic _topic;
    private Hub _hub;
    private MapEditor _mapEditor;
    private PlayerListEditor _playerListEditor;

    public Lobby(LobbyState state, IUserTopic topic, IGameFactory gameFactory)
    {
        if (state == null)
        {
            throw new ArgumentNullException(nameof(state));
        }

        if (string.IsNullOrEmpty(state.RoomId))
        {
            throw new ArgumentException("Room id should be initialized", nameof(state));
        }

        _state = state;
        _topic = topic ?? throw new ArgumentNullException(nameof(topic));
        _gameFactory = gameFactory ?? throw new ArgumentNullException(nameof(gameFactory));
        Initialize();
    }

    public TimeSpan EveryoneOfflinePeriod => _hub.EveryoneOfflinePeriod;

    public Task Handle(GameEnded @event)
    {
        Reset();
        return Task.CompletedTask;
    }

    public Task Notify(ISet<Guid> userIds, params IEvent[] events)
    {
        return _hub.Notify(userIds, events);
    }

    public Task NotifyEverybody(params IEvent[] @event)
    {
        return _hub.NotifyEverybody(@event);
    }

    public Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events)
    {
        return _hub.NotifyEverybodyExcept(userIds, events);
    }

    public Task<IRoomData> GetState(Guid userId)
    {
        EnsureUserIsOnline(userId);
        var state = new LobbyData
        {
            RoomId = _state.RoomId,
            Events = _hub.GetEvents(userId).Select(x => x.ToEnvelope()).ToList(),
            Players = _state.Players,
            PresenceStatuses = _state.Players
                .ToDictionary(x => x.UserId,
                    x => _hub.IsOnline(x.UserId) ? PresenceStatus.Online : PresenceStatus.Offline),
            Map = _state.MapEditor.Map,
            LeaderUserId = _hub.LeaderUserId
        };
        return Task.FromResult<IRoomData>(state);
    }

    public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
    {
        EnsureUserIsOnline(userId);
        EnsureGameIsNotStarted();

        _mapEditor.Generate(options);
        var mapUpdated = new MapUpdated(_state.RoomId, _state.MapEditor.Map);
        await _hub.NotifyEverybody(mapUpdated);
    }

    public async Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
    {
        EnsureUserIsOnline(userId);
        EnsureGameIsNotStarted();

        if (!_playerListEditor.Contains(userId))
        {
            return;
        }

        var updated = _playerListEditor.Update(userId, options);
        if (!updated)
        {
            return;
        }

        var playerUpdated = new PlayerUpdated(_state.RoomId, _playerListEditor[userId]);
        await _hub.NotifyEverybody(playerUpdated);
    }

    public async Task<IGame> StartGame(Guid userId)
    {
        EnsureUserIsOnline(userId);
        EnsureGameIsNotStarted();

        if (_state.Players.Count < 2)
        {
            throw UserException.Create(
                GameplayError.NotEnoughPlayers,
                "At least 2 players are required to start the game.");
        }

        _state.IsGameStarted = true;
        var game = await _gameFactory.CreateGame(userId);
        return game;
    }

    public async Task Connect(Guid userId, string connectionId)
    {
        EnsureGameIsNotStarted();

        if (!_playerListEditor.Contains(userId))
        {
            var player = _playerListEditor.Add(userId);
            var playerJoined = new PlayerJoined(_state.RoomId, player);
            if (_playerListEditor.Count == 1)
            {
                _mapEditor.Generate();
            }

            if (!_mapEditor.CanPlacePlanet)
            {
                var options = new GameOptionsData
                {
                    MapWidth = _mapEditor.MapWidth + 1,
                    MapHeight = _mapEditor.MapHeigth + 1,
                    NeutralPlanetsCount = _mapEditor.NeutralPlanetsCount
                };
                _mapEditor.Generate(options);
            }

            _mapEditor.PlacePlanet(userId);
            var mapUpdated = new MapUpdated(_state.RoomId, _state.MapEditor.Map);
            await _hub.NotifyEverybodyExcept(userId, playerJoined, mapUpdated);
        }

        await _hub.Connect(userId, connectionId);
    }

    public Task Disconnect(Guid userId, string connectionId)
    {
        return _hub.Disconnect(userId, connectionId);
    }

    private void Reset()
    {
        _state.IsGameStarted = false;
        _state.MapEditor = new MapEditorState();
        _state.Hub = new HubState { Id = _state.RoomId };
        _state.Players = new List<PlayerData>();
        Initialize();
    }

    private void Initialize()
    {
        _hub = new Hub(_state.Hub, _topic);
        _mapEditor = new MapEditor(
            _state.MapEditor,
            new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.PlayerPlanetDefaults),
            new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.NeutralPlanetDefaults));
        _playerListEditor = new PlayerListEditor(_state.Players);
    }

    private void EnsureUserIsOnline(Guid userId)
    {
        if (!_hub.IsOnline(userId))
        {
            throw new InvalidOperationException($"User {userId} is not online at {_state.RoomId}.");
        }
    }

    private void EnsureGameIsNotStarted()
    {
        if (!_state.IsGameStarted)
        {
            return;
        }

        var message = $"Game {_state.RoomId} is already in progress.";
        throw UserException.Create(GameplayError.GameIsAlreadyInProgress, message);
    }
}