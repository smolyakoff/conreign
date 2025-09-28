using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class UpdatePlayerOptionsCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
    public PlayerOptionsData Options { get; set; }
}