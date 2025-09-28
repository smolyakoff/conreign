using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class UpdateGameOptionsCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
    public GameOptionsData Options { get; set; }
}