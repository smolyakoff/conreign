using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class CancelFleetCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public int Index { get; set; }
    }
}