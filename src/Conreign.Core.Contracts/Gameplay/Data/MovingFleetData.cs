using System.Collections.Generic;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class MovingFleetData
    {
        public FleetData Fleet { get; set; } = new FleetData();
        public int Position { get; set; }
        public List<int> Route { get; set; } = new List<int>();
    }
}