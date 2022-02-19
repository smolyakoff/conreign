using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class JoinRoomCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
}