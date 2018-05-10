﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    public class Lobby : ILobby
    {
        private readonly LobbyState _state;
        private readonly ITopic _topic;
        private Hub _hub;
        private MapEditor _mapEditor;
        private PlayerListEditor _playerListEditor;

        public Lobby(LobbyState state, ITopic topic)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
            _topic = topic ?? throw new ArgumentNullException(nameof(topic));
            Initialize();
        }

        public TimeSpan EveryoneOfflinePeriod => _hub.EveryoneOfflinePeriod;

        public Task Handle(GameEnded @event)
        {
            Reset();
            return Task.CompletedTask;
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            EnsureUserIsOnline(userId);
            var state = new LobbyData
            {
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

        public Task SendMessage(Guid userId, TextMessageData textMessage)
        {
            EnsureUserIsOnline(userId);
            if (textMessage == null)
            {
                throw new ArgumentNullException(nameof(textMessage));
            }
            textMessage.EnsureIsValid<TextMessageData, TextMessageValidator>();
            var @event = new ChatMessageReceived(userId, textMessage);
            return _hub.NotifyEverybody(@event);
        }

        public async Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            EnsureUserIsOnline(userId);
            EnsureGameIsNotStarted();

            var validator = new GameOptionsValidator(_playerListEditor.HumansCount);
            validator.EnsureIsValid(options);

            var (botsAdded, botsRemoved) = _playerListEditor.AdjustBotCount(options.BotsCount);
            var mapSize = new MapSize(options.MapWidth, options.MapHeight);
           _mapEditor.GenerateMap(
               mapSize,
               _playerListEditor.PlayerIds.ToHashSet(), 
               options.NeutralPlanetsCount);
            var playerListChanged = new PlayerListChanged(
                botsAdded,
                botsRemoved.Select(x => x.UserId).ToList());
            var mapUpdated = new MapUpdated(_state.Map);
            await _hub.NotifyEverybody(playerListChanged, mapUpdated);
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
            var playerUpdated = new PlayerUpdated(_playerListEditor[userId]);
            await _hub.NotifyEverybody(playerUpdated);
        }

        public Task<InitialGameData> InitializeGame(Guid userId)
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
            var data = new InitialGameData(
                userId,
                _state.Map,
                _state.Players,
                _state.Hub.Members,
                _state.Hub.JoinOrder
            );
            return Task.FromResult(data);
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
                var playerJoined = new PlayerJoined(player);
                var mapUpdated = new MapUpdated(_state.Map);
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
            _state.Hub = new HubState();
            _state.Players = new List<PlayerData>();
            Initialize();
        }

        private void Initialize()
        {
            _mapEditor = new MapEditor(
                _state.Map,
                new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.PlayerPlanetDefaults),
                new UniformRandomPlanetGenerator(UniformRandomPlanetGeneratorOptions.NeutralPlanetDefaults));
            _playerListEditor = new PlayerListEditor(_state.Players);
            _playerListEditor.AdjustBotCount(GameOptionsData.DefaultBotsCount);
            _hub = new Hub(_state.Hub, _topic, new BotUserIdSet(_playerListEditor));
            _mapEditor.GenerateMap(
                _playerListEditor.PlayerIds.ToHashSet(), 
                GameOptionsData.DefaultNeutralPlayersCount);
        }

        private void EnsureUserIsOnline(Guid userId)
        {
            if (!_hub.IsOnline(userId))
            {
                throw new InvalidOperationException($"User {userId} is not online.");
            }
        }

        private void EnsureGameIsNotStarted()
        {
            if (!_state.IsGameStarted)
            {
                return;
            }
            const string message = "Game is already in progress.";
            throw UserException.Create(GameplayError.GameIsAlreadyInProgress, message);
        }

        private class BotUserIdSet : IReadOnlySet<Guid>
        {
            private readonly PlayerListEditor _playerListEditor;

            public BotUserIdSet(PlayerListEditor playerListEditor)
            {
                _playerListEditor = playerListEditor;
            }

            public bool Contains(Guid userId)
            {
                return _playerListEditor.ContainsBotWithUserId(userId);
            }

            public IEnumerator<Guid> GetEnumerator()
            {
                return _playerListEditor.BotIds.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}