using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Errors;
using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Contracts.Presence;
using Conreign.Core;
using Conreign.Core.Editor;
using Conreign.Core.Utility;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Gameplay.Validators;
using Conreign.Server.Presence;

namespace Conreign.Server.Gameplay
{
    public class Lobby : ILobby, IEventHandler<GameEnded>
    {
        private readonly LobbyState _state;
        private readonly IBroadcastTopic _topic;
        private Hub _hub;
        private MapEditor _mapEditor;
        private PlayerListEditor _playerListEditor;

        public Lobby(LobbyState state, IBroadcastTopic topic)
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
                Map = _state.Map,
                LeaderUserId = _hub.LeaderUserId
            };
            return Task.FromResult<IRoomData>(state);
        }

        public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            EnsureUserIsOnline(userId);
            EnsureGameIsNotStarted();

            var validator = new GameOptionsValidator(_playerListEditor.HumansCount);
            validator.EnsureIsValid(options);

           _playerListEditor.AdjustBotCount(options.BotsCount);
           var mapSize = new MapSize(options.MapWidth, options.MapHeight);
           _mapEditor.GenerateMap(
               mapSize,
               _playerListEditor.PlayerIds.ToHashSet(), 
               options.NeutralPlanetsCount);
            var mapUpdated = new MapUpdated(_state.RoomId, _state.Map);
            await _hub.NotifyEverybody(mapUpdated);
        }

        public async Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            EnsureUserIsOnline(userId);
            EnsureGameIsNotStarted();

            var updated = _playerListEditor.UpdateHumanOptions(userId, options);
            if (!updated)
            {
                return;
            }
            var playerUpdated = new PlayerUpdated(_state.RoomId, _playerListEditor[userId]);
            await _hub.NotifyEverybody(playerUpdated);
        }

        public Task StartGame(Guid userId)
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
            return Task.CompletedTask;
        }

        public async Task Connect(Guid userId, Guid connectionId)
        {
            EnsureGameIsNotStarted();

            if (!_playerListEditor.ContainsPlayerWithUserId(userId))
            {
                var planetPlaced = _mapEditor.TryPlacePlanet(userId);
                if (!planetPlaced)
                {
                    throw UserException.Create(
                        GameplayError.NoFreeMapCells,
                        "No free map cells available.");
                }
                var player = _playerListEditor.AddHuman(userId);
                var playerJoined = new PlayerJoined(_state.RoomId, player);
                var mapUpdated = new MapUpdated(_state.RoomId, _state.Map);
                await _hub.NotifyEverybodyExcept(userId, playerJoined, mapUpdated);
            }
            await _hub.Connect(userId, connectionId);
        }

        public Task Disconnect(Guid userId, Guid connectionId)
        {
            return _hub.Disconnect(userId, connectionId);
        }

        private void Reset()
        {
            _state.IsGameStarted = false;
            _state.Map = new MapData();
            _state.Hub = new HubState {Id = _state.RoomId};
            _state.Players = new List<PlayerData>();
            Initialize();
        }

        private void Initialize()
        {
            _hub = new Hub(_state.Hub, _topic);
            _mapEditor = new MapEditor(
                _state.Map,
                new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.PlayerPlanetDefaults),
                new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.NeutralPlanetDefaults));
            _playerListEditor = new PlayerListEditor(_state.Players);
            _mapEditor.GenerateMap(GameOptionsData.DefaultNeutralPlayersCount);
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
}