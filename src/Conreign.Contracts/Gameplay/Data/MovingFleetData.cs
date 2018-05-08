using System;
using System.Collections.Generic;

namespace Conreign.Contracts.Gameplay.Data
{
    public class MovingFleetData
    {
        public MovingFleetData(Guid ownerUserId, FleetData fleet, List<int> route)
        {
            if (ownerUserId == Guid.Empty)
            {
                throw new ArgumentException("Owner user id should not be empty.", nameof(ownerUserId));
            }
            OwnerUserId = ownerUserId;
            Fleet = fleet ?? throw new ArgumentNullException(nameof(fleet));
            Route = route ?? throw new ArgumentNullException(nameof(route));
            if (route.Count < 2)
            {
                throw new ArgumentException("Route should contain at least 2 positions.", nameof(route));
            }
            Position = fleet.From;
        }

        public Guid OwnerUserId { get; }
        public FleetData Fleet { get; }
        public int Position { get; set; }
        public List<int> Route { get; }
    }
}