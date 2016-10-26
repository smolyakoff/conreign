using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    public class GetRoomStateHandler : IAsyncRequestHandler<GetRoomStateCommand, IRoomData>
    {
        private readonly IHandlerContext _context;

        public GetRoomStateHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<IRoomData> Handle(GetRoomStateCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            return await player.GetState();
        }
    }
}
