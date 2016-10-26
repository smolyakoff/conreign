using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Exceptions;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Errors;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Gameplay.Editor;
using Conreign.Core.Presence;

namespace Conreign.Core.Gameplay
{
    public class Lobby : ILobby, IEventHandler<GameEnded>
    {
        private readonly LobbyState _state;
        private readonly IUserTopic _topic;
        private Hub _hub;
        private MapEditor _mapEditor;
        private PlayerListEditor _playerListEditor;
        private readonly IGameFactory _gameFactory;

        public Lobby(LobbyState state, IUserTopic topic, IGameFactory gameFactory)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            if (gameFactory == null)
            {
                throw new ArgumentNullException(nameof(gameFactory));
            }
            if (string.IsNullOrEmpty(state.RoomId))
            {
                throw new ArgumentException("Room id should be initialized", nameof(state));
            }
            _state = state;
            _topic = topic;
            _gameFactory = gameFactory;
            Initialize();
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
                Events = _hub.GetEvents(userId).Select(x => new MessageEnvelope {Payload = x}).ToList(),
                Players = _state.Players,
                PlayerStatuses = _state.Players
                    .ToDictionary(x => x.UserId,  x => _hub.HasMemberOnline(x.UserId) ? PresenceStatus.Online : PresenceStatus.Offline),
                Map = _state.MapEditor.Map,
                LeaderUserId = _hub.LeaderUserId,
                NeutralPlanetsCount = _state.MapEditor.NeutralPlanetsCount
            };
            return Task.FromResult<IRoomData>(state);
        }

        public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            EnsureUserIsOnline(userId);
            EnsureGameIsNotStarted();

            var current = new GameOptionsData
            {
                MapWidth = _mapEditor.MapWidth,
                MapHeight = _mapEditor.MapHeigth,
                NeutralPlanetsCount = _mapEditor.NeutralPlanetsCount
            };
            if (current == options)
            {
                return;
            }
            _mapEditor.Generate(options);
            var mapUpdated = new MapUpdated(_state.MapEditor.Map);
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
            var playerUpdated = new PlayerUpdated(_playerListEditor[userId]);
            await _hub.NotifyEverybody(playerUpdated);
        }

        public Task GenerateMap(Guid userId)
        {
            EnsureUserIsOnline(userId);
            EnsureGameIsNotStarted();

            _mapEditor.Generate();
            var mapUpdated = new MapUpdated(_state.MapEditor.Map);
            return _hub.NotifyEverybody(mapUpdated);
        }

        public async Task<IGame> StartGame(Guid userId)
        {
            EnsureUserIsOnline(userId);
            EnsureGameIsNotStarted();

            _state.IsGameStarted = true;
            var game = await _gameFactory.CreateGame(userId);
            return game;
        }

        public async Task Connect(Guid userId, Guid connectionId)
        {
            EnsureGameIsNotStarted();

            if (!_playerListEditor.Contains(userId))
            {
                var player = _playerListEditor.Add(userId);
                var playerJoined = new PlayerJoined(player);
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
                var mapUpdated = new MapUpdated(_state.MapEditor.Map);
                await _hub.NotifyEverybody(playerJoined, mapUpdated);
            }
            await _hub.Connect(userId, connectionId);
        }

        public Task Disconnect(Guid userId, Guid connectionId)
        {
            return _hub.Disconnect(userId, connectionId);
        }

        public Task Handle(GameEnded @event)
        {
            Reset();
            return Task.CompletedTask;
        }

        private void Reset()
        {
            _state.IsGameStarted = false;
            _state.MapEditor = new MapEditorState();
            _state.Hub = new HubState();
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
            if (!_hub.HasMemberOnline(userId))
            {
                throw new InvalidOperationException($"User ${userId} is not a online at {_state.RoomId}.");
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