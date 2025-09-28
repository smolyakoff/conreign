using Conreign.Server.Contracts.Shared.Gameplay.Data;
using MediatR;

namespace Conreign.Server.Contracts.Client.Messages;

public class LaunchFleetCommand : IRequest<Unit>
{
    public string RoomId { get; set; }
    public FleetData Fleet { get; set; }
}