using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class StartGameCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
}