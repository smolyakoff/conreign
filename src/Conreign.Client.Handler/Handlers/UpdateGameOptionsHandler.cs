using System.Threading.Tasks;
using Conreign.Client.Contracts.Messages;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class UpdateGameOptionsHandler : ICommandHandler<UpdateGameOptionsCommand, Unit>
    {
        public async Task<Unit> Handle(CommandEnvelope<UpdateGameOptionsCommand, Unit> message)
        {
            var context = message.Context;
            var command = message.Command;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            await player.UpdateGameOptions(command.Options);
            return Unit.Value;
        }
    }
}