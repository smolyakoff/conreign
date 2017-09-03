using Conreign.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class GetRoomStateCommand : IRequest<IRoomData>
    {
        public string RoomId { get; set; }
    }
}