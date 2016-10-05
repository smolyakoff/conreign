using System;

namespace Conreign.Core.Gameplay
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