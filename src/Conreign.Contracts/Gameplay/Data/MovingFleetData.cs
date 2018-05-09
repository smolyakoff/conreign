using System;
using System.Collections.Generic;

namespace Conreign.Contracts.Gameplay.Data
{
    public class MovingFleetData
    {
        public MovingFleetData(FleetData fleet, List<int> route)
        {
            Fleet = fleet ?? throw new ArgumentNullException(nameof(fleet));
            Route = route ?? throw new ArgumentNullException(nameof(route));
            if (route.Count < 2)
            {
                throw new ArgumentException("Route should contain at least 2 positions.", nameof(route));
            }
            Position = fleet.From;
        }

        public FleetData Fleet { get; }
        public int Position { get; set; }
        public List<int> Route { get; }
    }
}