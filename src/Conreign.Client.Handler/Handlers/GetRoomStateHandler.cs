using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using Conreign.Core.Contracts.Client.Messages;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Client.Handler.Handlers
{
    internal class GetRoomStateHandler : ICommandHandler<GetRoomStateCommand, IRoomData>
    {
        public async Task<IRoomData> Handle(CommandEnvelope<GetRoomStateCommand, IRoomData> message)
        {
            var context = message.Context;
            var command = message.Command;
            var player = await context.User.JoinRoom(command.RoomId, context.Connection.Id);
            return await player.GetState();
        }
    }
}