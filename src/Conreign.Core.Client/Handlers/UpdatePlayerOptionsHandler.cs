using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Core.Client.Handlers
{
    internal class UpdatePlayerOptionsHandler : IAsyncRequestHandler<UpdatePlayerOptionsCommand, Unit>
    {
        private readonly IHandlerContext _context;

        public UpdatePlayerOptionsHandler(IHandlerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            _context = context;
        }

        public async Task<Unit> Handle(UpdatePlayerOptionsCommand message)
        {
            var player = await _context.User.JoinRoom(message.RoomId, _context.Connection.Id);
            await player.UpdateOptions(message.Options);
            return Unit.Value;
        }
    }
}