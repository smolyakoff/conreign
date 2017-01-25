using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class JoinRoomCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
    }
}