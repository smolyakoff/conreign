using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class EndTurnCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
    }
}