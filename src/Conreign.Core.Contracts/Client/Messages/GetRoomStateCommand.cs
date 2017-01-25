using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class GetRoomStateCommand : IRequest<IRoomData>
    {
        public string RoomId { get; set; }
    }
}