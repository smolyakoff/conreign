using Conreign.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Client.Contracts.Messages
{
    public class LaunchFleetCommand : IRequest<Unit>
    {
        public string RoomId { get; set; }
        public FleetData Fleet { get; set; }
    }
}