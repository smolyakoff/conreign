using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Client.SignalR.Proxies
{
    internal class PlayerProxy : IPlayer
    {
        private readonly SignalRConnectionContext _context;
        private readonly string _roomId;

        public PlayerProxy(SignalRConnectionContext context, string roomId)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            _context = context;
            _roomId = roomId;
        }

        public Task UpdateOptions(PlayerOptionsData options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var command = new UpdatePlayerOptionsCommand
            {
                RoomId = _roomId,
                Options = options
            };
            return _context.Send(command);
        }

        public Task UpdateGameOptions(GameOptionsData options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var command = new UpdateGameOptionsCommand
            {
                RoomId = _roomId,
                Options = options
            };
            return _context.Send(command);
        }

        public Task StartGame()
        {
            var command = new StartGameCommand
            {
                RoomId = _roomId
            };
            return _context.Send(command);
        }

        public Task LaunchFleet(FleetData fleet)
        {
            if (fleet == null)
            {
                throw new ArgumentNullException(nameof(fleet));
            }
            var command = new LaunchFleetCommand
            {
                RoomId = _roomId,
                Fleet = fleet
            };
            return _context.Send(command);
        }

        public Task CancelFleet(FleetCancelationData fleetCancelation)
        {
            if (fleetCancelation == null)
            {
                throw new ArgumentNullException(nameof(fleetCancelation));
            }
            var command = new CancelFleetCommand
            {
                RoomId = _roomId,
                Index = fleetCancelation.Index
            };
            return _context.Send(command);
        }

        public Task EndTurn()
        {
            var command = new EndTurnCommand {RoomId = _roomId};
            return _context.Send(command);
        }

        public Task Write(TextMessageData textMessage)
        {
            if (textMessage == null)
            {
                throw new ArgumentNullException(nameof(textMessage));
            }
            var command = new WriteCommand
            {
                RoomId = _roomId,
                Text = textMessage.Text
            };
            return _context.Send(command);
        }

        public Task<IRoomData> GetState()
        {
            var command = new GetRoomStateCommand
            {
                RoomId = _roomId
            };
            return _context.Send(command);
        }
    }
}