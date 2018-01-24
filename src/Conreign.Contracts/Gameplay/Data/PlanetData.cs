using System;

namespace Conreign.Contracts.Gameplay.Data
{
    public class PlanetData : IPlanetData
    {
        public string Name { get; set; }
        public int ProductionRate { get; set; }
        public double Power { get; set; }
        public int Ships { get; set; }
        public Guid? OwnerId { get; set; }
    }
}