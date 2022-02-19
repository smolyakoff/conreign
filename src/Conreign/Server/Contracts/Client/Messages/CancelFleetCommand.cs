using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class CancelFleetCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
    public int Index { get; set; }
}