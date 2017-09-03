using System.Threading.Tasks;
using Conreign.Client.Contracts.Messages;
using MediatR;

namespace Conreign.Client.Handler.Handlers
{
    internal class JoinRoomHandler : ICommandHandler<JoinRoomCommand, Unit>
    {
        public async Task<Unit> Handle(CommandEnvelope<JoinRoomCommand, Unit> message)
        {
            var context = message.Context;
            var command = message.Command;
            await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            return Unit.Value;
        }
    }
}