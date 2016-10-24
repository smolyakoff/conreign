using System;
using System.Threading.Tasks;
using Conreign.Core.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    public class StartGameHandler : IAsyncRequestHandler<StartGameCommand, Unit>
    {
        private readonly IHandlerContext _context;

        public StartGameHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<Unit> Handle(StartGameCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            await player.StartGame();
            return Unit.Value;
        }
    }
}
