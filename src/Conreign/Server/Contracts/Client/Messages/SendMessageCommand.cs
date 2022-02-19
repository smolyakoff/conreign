using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class SendMessageCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
    public string Text { get; set; }
}