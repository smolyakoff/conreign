using System;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class PlanetState
    {
        public string Name { get; set; }
        public int ProductionRate { get; set; }
        public double Power { get; set; }
        public int Ships { get; set; }
        public Guid? OwnerId { get; set; }
    }
}