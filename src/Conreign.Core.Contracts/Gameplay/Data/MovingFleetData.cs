using System.Collections.Generic;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class MovingFleetData
    {
        public FleetData Fleet { get; set; } = new FleetData();
        public long Position { get; set; }
        public List<long> Route { get; set; } = new List<long>();
    }
}
