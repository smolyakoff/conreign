using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using Conreign.Core.Contracts.Client.Messages;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class UpdatePlayerOptionsHandler : ICommandHandler<UpdatePlayerOptionsCommand, Unit>
    {
        public async Task<Unit> Handle(CommandEnvelope<UpdatePlayerOptionsCommand, Unit> message)
        {
            var context = message.Context;
            var command = message.Command;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            await player.UpdateOptions(command.Options);
            return Unit.Value;
        }
    }
}