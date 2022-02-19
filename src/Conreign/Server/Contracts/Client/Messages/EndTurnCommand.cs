using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class EndTurnCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
}