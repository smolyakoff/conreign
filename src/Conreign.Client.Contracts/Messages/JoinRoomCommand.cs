using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class JoinRoomCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
    }
}