using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Client.Messages
{
    public class GetRoomStateCommand : IAsyncRequest<IRoomData>
    {
        public string RoomId { get; set; }
    }
}
