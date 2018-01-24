using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Core.Utility;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Communication.Events;
using Conreign.Server.Contracts.Gameplay;
using Conreign.Server.Gameplay.Validators;
using Conreign.Server.Presence;

namespace Conreign.Server.Gameplay
{
    public class Player : IPlayer,
        IEventHandler<GameStarted>,
        IEventHandler<GameEnded>,
        IEventHandler<Connected>,
        IEventHandler<Disconnected>
    {
        private readonly PlayerState _state;
        private readonly Func<IRoom> _roomProvider;

        public Player(PlayerState state, Func<IRoom> roomProvider)
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
            _state = state;
            _roomProvider = roomProvider ?? throw new ArgumentNullException(nameof(roomProvider));
        }

        private IRoom Room => _roomProvider();

        public Task Handle(Connected @event)
        {
            return Room.Connect(_state.UserId, @event.ConnectionId);
        }

        public Task Handle(Disconnected @event)
        {
            return Room.Disconnect(_state.UserId, @event.ConnectionId);
        }

        public Task Handle(GameEnded @event)
        {
            _state.RoomMode = RoomMode.Lobby;
            return Task.CompletedTask;
        }

        public Task Handle(GameStarted @event)
        {
            _state.RoomMode = RoomMode.Game;
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
            var gameData = await lobby.InitializeGame(_state.UserId);
            _state.RoomMode = RoomMode.Game;
            var game = EnsureIsInGame();
            await game.Start(_state.UserId, gameData);
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

        public Task SendMessage(TextMessageData textMessage)
        {
            if (textMessage == null)
            {
                throw new ArgumentNullException(nameof(textMessage));
            }
            textMessage.EnsureIsValid<TextMessageData, TextMessageValidator>();
            var @event = new ChatMessageReceived(_state.RoomId, _state.UserId, textMessage);
            return Room.NotifyEverybody(@event);
        }

        public async Task<IRoomData> GetState()
        {
            var state = await Room.GetState(_state.UserId);
            return state;
        }

        private ILobby EnsureIsInLobby()
        {
            var lobby = Room as ILobby;
            if (lobby == null)
            {
                throw new InvalidOperationException("Player should be in lobby to perform this action.");
            }
            return lobby;
        }

        private IGame EnsureIsInGame()
        {
            var game = Room as IGame;
            if (game == null)
            {
                throw new InvalidOperationException("Player should be in game to perform this action.");
            }
            return game;
        }
    }
}