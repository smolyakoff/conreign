using System;
using System.Threading.Tasks;
using Conreign.Core.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class UpdateGameOptionsHandler : IAsyncRequestHandler<UpdateGameOptionsCommand, Unit>
    {
        private readonly IHandlerContext _context;

        public UpdateGameOptionsHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<Unit> Handle(UpdateGameOptionsCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            await player.UpdateGameOptions(message.Options);
            return Unit.Value;
        }
    }
}
