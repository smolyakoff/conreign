using Conreign.Core.Contracts.Gameplay.Data;
using MediatR;

namespace Conreign.Core.Contracts.Client.Messages
{
    public class LaunchFleetCommand : IAsyncRequest
    {
        public string RoomId { get; set; }
        public FleetData Fleet { get; set; }
    }
}
