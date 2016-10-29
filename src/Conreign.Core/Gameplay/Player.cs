using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay.Validators;
using Conreign.Core.Utility;

namespace Conreign.Core.Gameplay
{
    public class Player : IPlayer,
        IEventHandler<GameStarted.Server>,
        IEventHandler<GameEnded>,
        IEventHandler<Connected>,
        IEventHandler<Disconnected>
    {
        private readonly PlayerState _state;

        public Player(PlayerState state)
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
        }

        public Task Handle(Connected @event)
        {
            return _state.Room.Connect(_state.UserId, @event.ConnectionId);
        }

        public Task Handle(Disconnected @event)
        {
            return _state.Room.Disconnect(_state.UserId, @event.ConnectionId);
        }

        public Task Handle(GameEnded @event)
        {
            if (_state.Game != null)
            {
                _state.Game = null;
            }
            return Task.CompletedTask;
        }

        public Task Handle(GameStarted.Server @event)
        {
            if (_state.Game == null)
            {
                _state.Game = @event.Game;
            }
            return Task.CompletedTask;
        }

        public Task UpdateOptions(PlayerOptionsData options)
        {
            var lobby = EnsureIsInLobby();
            return lobby.UpdatePlayerOptions(_state.UserId, options);
        }

        public Task UpdateGameOptions(GameOptionsData options)
        {
            var lobby = EnsureIsInLobby();
            return lobby.UpdateGameOptions(_state.UserId, options);
        }

        public async Task StartGame()
        {
            var lobby = EnsureIsInLobby();
            var game = await lobby.StartGame(_state.UserId);
            _state.Game = game;
            await game.NotifyEverybody(new GameStarted());
        }

        public Task LaunchFleet(FleetData fleet)
        {
            var game = EnsureIsInGame();
            return game.LaunchFleet(_state.UserId, fleet);
        }

        public Task CancelFleet(FleetCancelationData fleetCancelation)
        {
            var game = EnsureIsInGame();
            return game.CancelFleet(_state.UserId, fleetCancelation);
        }

        public Task EndTurn()
        {
            var game = EnsureIsInGame();
            return game.EndTurn(_state.UserId);
        }

        public Task Write(TextMessageData textMessage)
        {
            if (textMessage == null)
            {
                throw new ArgumentNullException(nameof(textMessage));
            }
            textMessage.EnsureIsValid<TextMessageData, TextMessageValidator>();
            var @event = new ChatMessageReceived(_state.UserId, textMessage);
            return _state.Room.NotifyEverybody(@event);
        }

        public async Task<IRoomData> GetState()
        {
            var state = await _state.Room.GetState(_state.UserId);
            return state;
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