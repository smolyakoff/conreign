using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class Lobby : ILobby
    {
        private readonly LobbyState _state;

        private readonly Hub _hub;
        private readonly MapEditor _mapEditor;
        private readonly PlayerListEditor _playerListEditor;
        private readonly IGameFactory _gameFactory;

        public Lobby(LobbyState state, IGameFactory gameFactory)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
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
            _hub = new Hub(state.Hub);
            _mapEditor = new MapEditor(
                state.MapEditor, 
                new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.PlayerPlanetDefaults),
                new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.NeutralPlanetDefaults));
            _playerListEditor = new PlayerListEditor(_state.Players);
            _gameFactory = gameFactory;
        }

        public Task Notify(ISet<Guid> users, params IEvent[] events)
        {
            return _hub.Notify(users, events);
        }

        public Task NotifyEverybody(params IEvent[] @event)
        {
            return _hub.NotifyEverybody(@event);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events)
        {
            return _hub.NotifyEverybodyExcept(users, events);
        }

        public async Task Join(Guid userId, IPublisher<IEvent> publisher)
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
            await _hub.Join(userId, publisher);
        }

        public async Task Leave(Guid userId)
        {
            await _hub.Leave(userId);
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            if (!_hub.HasMemberOnline(userId))
            {
                throw new InvalidOperationException("Operation is only allowed for online members.");
            }
            var state = new LobbyData
            {
                Events = _hub.GetEvents(userId).ToList(),
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
            EnsureGameIsNotStarted();
            _mapEditor.Generate();
            var mapUpdated = new MapUpdated(_state.MapEditor.Map);
            _hub.NotifyEverybody(mapUpdated);
            return Task.CompletedTask;
        }

        public async Task<IGame> StartGame(Guid userId)
        {
            EnsureGameIsNotStarted();
            _state.IsGameStarted = true;
            var game = await _gameFactory.CreateGame();
            await _hub.NotifyEverybody(new GameStarted(), new GameStarted.System(game));
            return game;
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