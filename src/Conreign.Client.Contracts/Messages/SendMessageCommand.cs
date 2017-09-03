using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class SendMessageCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public string Text { get; set; }
    }
}