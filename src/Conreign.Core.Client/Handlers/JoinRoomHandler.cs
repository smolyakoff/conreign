using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    public class JoinRoomHandler : IAsyncRequestHandler<JoinRoomCommand, Unit>
    {
        private readonly IHandlerContext _context;

        public JoinRoomHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<Unit> Handle(JoinRoomCommand message)
        {
            await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            return Unit.Value;
        }
    }
}
