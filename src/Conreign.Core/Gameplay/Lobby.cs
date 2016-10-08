using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Presence;

namespace Conreign.Core.Gameplay
{
    public class Lobby : ILobby
    {
        private readonly Hub _hub;
        private readonly LobbyState _state;
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
            _hub = new Hub(state.Hub);
            _state = state;
            _gameFactory = gameFactory;
        }

        public Task Notify(ISet<Guid> users, params IClientEvent[] events)
        {
            return _hub.Notify(users, events);
        }

        public Task NotifyEverybody(params IClientEvent[] @event)
        {
            return _hub.NotifyEverybody(@event);
        }

        public Task NotifyEverybodyExcept(ISet<Guid> users, params IClientEvent[] events)
        {
            return _hub.NotifyEverybodyExcept(users, events);
        }

        public async Task Join(Guid userId, IClientObserver observer)
        {
            await _hub.Join(userId, observer);
            if (!_state.Players.Exists(x => x.UserId == userId))
            {
                var color = Colors
                    .RandomWithPopularFirst()
                    .FirstOrDefault(c => _state.Players.All(p => p.Color != c));
                var player = new PlayerData
                {
                    UserId = userId,
                    Color = color,
                    Nickname = null
                };
                _state.Players.Add(player);
                var @event = new PlayerJoined { Player = player };
                await _hub.NotifyEverybody(@event);
            }
        }

        public async Task Leave(Guid userId)
        {
            await _hub.Leave(userId);
        }

        public Task<IRoomData> GetState(Guid userId)
        {
            if (!_hub.HasMemberOnline(userId))
            {
                throw new InvalidOperationException("Operation is only allowed member users.");
            }
            var state = new LobbyData
            {
                Events = _hub.GetEvents(userId).ToList(),
                Players = _state.Players,
                PlayerStatuses = _state.Players
                    .ToDictionary(x => x.UserId,  x => _hub.HasMemberOnline(x.UserId) ? PresenceStatus.Online : PresenceStatus.Offline),
                GameOptions = _state.GameOptions,
                // ReSharper disable once PossibleInvalidOperationException
                LeaderUserId = _hub.LeaderUserId.Value,
                Map = _state.Map
            };
            return Task.FromResult<IRoomData>(state);
        }

        public Task UpdateGameOptions(Guid userId, GameOptionsData options)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options)
        {
            throw new NotImplementedException();
        }

        public Task<IGame> StartGame(Guid userId)
        {
            throw new NotImplementedException();
        }

        internal Task RegenerateMap()
        {
            return Task.CompletedTask;
        }
    }
}