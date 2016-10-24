using System;
using System.Threading.Tasks;
using Conreign.Core.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class EndTurnHandler : IAsyncRequestHandler<EndTurnCommand, Unit>
    {
        private readonly IHandlerContext _context;

        public EndTurnHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<Unit> Handle(EndTurnCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            await player.EndTurn();
            return Unit.Value;
        }
    }
}
