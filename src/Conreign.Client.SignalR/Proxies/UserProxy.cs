using System;
using System.Threading.Tasks;
using Conreign.Client.Contracts.Messages;
using Conreign.Contracts.Gameplay;

namespace Conreign.Client.SignalR.Proxies
{
    internal class UserProxy : IUserClient
    {
        private readonly SignalRSender _context;

        public UserProxy(SignalRSender context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IPlayerClient> JoinRoom(string roomId, Guid connectionId)
        {
            var command = new JoinRoomCommand {RoomId = roomId};
            await _context.Send(command);
            return new PlayerProxy(_context, roomId);
        }
    }
}