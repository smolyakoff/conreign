using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay
{
    public class Player : IConnectablePlayer, IObserver
    {
        private readonly PlayerState _state;
        private readonly IObserver _self;

        public Player(PlayerState state, IObserver self = null)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (string.IsNullOrEmpty(state.RoomId))
            {
                throw new ArgumentException("Room id should not be empty", nameof(state));
            }
            if (state.UserId == default(Guid))
            {
                throw new ArgumentException("User id should be initialized", nameof(state));
            }
            if (state.Room == null)
            {
                throw new ArgumentException("Room should be initialized", nameof(state));
            }
            _state = state;
            _self = self ?? this;
        }

        public Task UpdateOptions(PlayerOptionsData options)
        {
            var lobby = EnsureIsInLobby();
            return lobby.UpdatePlayerOptions(_state.UserId, options);
        }

        public Task UpdateGameOptions(GameOptionsData options)
        {
            throw new NotImplementedException();
        }

        public async Task StartGame()
        {
            var lobby = EnsureIsInLobby();
            var game = await lobby.StartGame(_state.UserId);
            _state.Room = game;
        }

        public Task LaunchFleet(FleetData fleet)
        {
            var game = EnsureIsInGame();
            return game.LaunchFleet(_state.UserId, fleet);
        }

        public Task EndTurn()
        {
            throw new NotImplementedException();
        }

        public Task Write(string text)
        {
            var @event = new ChatMessageReceived
            {
                SenderId = _state.UserId,
                Text = text
            };
            return _state.Room.NotifyEverybody(@event);
        }

        public Task<IRoomState> GetState()
        {
            return _state.Room.GetState(_state.UserId);
        }

        public Task Notify(object @event)
        {
            return this == _self ? Task.CompletedTask : _self.Notify(@event);
        }

        public async Task Connect(Guid connectionId)
        {
            _state.ConnectionIds.Add(connectionId);
            var isFirstConnection = _state.ConnectionIds.Count == 1;
            if (isFirstConnection)
            {
                await _state.Room.Join(_state.UserId, _self);
            }
        }

        public async Task Disconnect(Guid connectionId)
        {
            _state.ConnectionIds.Remove(connectionId);
            if (_state.ConnectionIds.Count == 0)
            {
                await _state.Room.Leave(_state.UserId);
            }
        }

        private ILobby EnsureIsInLobby()
        {
            var lobby = _state.Room as ILobby;
            if (lobby == null)
            {
                throw new InvalidOperationException("Player should be in lobby to perform this action.");
            }
            return lobby;
        }

        private IGame EnsureIsInGame()
        {
            var game = _state.Room as IGame;
            if (game == null)
            {
                throw new InvalidOperationException("Player should be in game to perform this action.");
            }
            return game;
        }
    }
}