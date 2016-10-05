using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Commands;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Gameplay
{
    public class Player : IPlayer, IChannel
    {
        private readonly PlayerState _state;
        private readonly IChannel _self;
        private readonly IConnectable _connectable;

        public Player(PlayerState state, IConnectable connectable, IChannel self = null)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (connectable == null)
            {
                throw new ArgumentNullException(nameof(connectable));
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
            _connectable = connectable;
            _self = self ?? this;
        }

        public async Task JoinRoom(JoinRoomCommand command)
        {
            var connectCommand = new ConnectCommand
            {
                ConnectionId = command.ConnectionId,
                Connection = _self,
            };
            await _connectable.Connect(connectCommand);

            _state.ConnectionIds.Add(command.ConnectionId);
            var isFirstConnection = _state.ConnectionIds.Count == 1;
            if (isFirstConnection)
            {
                var joinCommand = new JoinCommand
                {
                    Observer = _self,
                    UserId = _state.UserId
                };
                await _state.Room.Join(joinCommand);
            }
        }

        public Task UpdateOptions(UpdatePlayerOptionsCommand command)
        {
            var lobby = EnsureBeingInLobby("It's not allowed to change name during the game");
            return lobby.UpdatePlayerOptions(command);
        }

        public Task UpdateGameOptions()
        {
            throw new NotImplementedException();
        }

        public async Task StartGame(StartGameCommand command)
        {
            var lobby = EnsureBeingInLobby("Should be in lobby to start the game");
            var game = await lobby.StartGame(command);
            _state.Room = game;
        }

        public Task LaunchFleet()
        {
            throw new NotImplementedException();
        }

        public Task EndTurn()
        {
            throw new NotImplementedException();
        }

        public Task Write(WriteCommand command)
        {
            var @event = new ChatMessageReceivedEvent
            {
                SenderId = command.UserId,
                Text = command.Text
            };
            var notifyCommand = new NotifyEverybodyCommand(@event);
            return _state.Room.NotifyEverybody(notifyCommand);
        }

        public Task<IRoomState> GetState()
        {
            return _state.Room.GetState(_state.UserId);
        }

        public Task Disconnect(DisconnectCommand command)
        {
            throw new NotImplementedException();
        }

        public Task Notify(object @event)
        {
            return this == _self ? Task.CompletedTask : _self.Notify(@event);
        }

        private ILobby EnsureBeingInLobby(string message)
        {
            var lobby = _state.Room as ILobby;
            if (lobby == null)
            {
                throw new InvalidOperationException(message);
            }
            return lobby;
        }
    }
}