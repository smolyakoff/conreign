using System;

namespace Conreign.Core.Client.Commands
{
    public class LaunchFleetCommand
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; }
        public string SourcePlanet { get; set; }
        public string DestinationPlanet { get; set; }
        public int ShipsAmount { get; set; }
    }
}
