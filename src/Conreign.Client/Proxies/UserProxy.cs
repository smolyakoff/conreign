using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Client.Proxies
{
    internal class UserProxy : IUser
    {
        private readonly GameObjectContext _context;

        public UserProxy(GameObjectContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<IPlayer> JoinRoom(string roomId, Guid connectionId)
        {
            var command = new JoinRoomCommand {RoomId = roomId};
            await _context.Send(command);
            return new PlayerProxy(_context, roomId);
        }
    }
}