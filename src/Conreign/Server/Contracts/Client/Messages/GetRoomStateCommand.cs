using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class GetRoomStateCommand : IRequest<IRoomData>
{
    public string RoomId { get; set; }
}