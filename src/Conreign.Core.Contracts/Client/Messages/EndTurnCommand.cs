using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class EndTurnCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
    }
}