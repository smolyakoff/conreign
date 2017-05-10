using System;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.AI.Battle
{
    public class ReadOnlyPlanetData
    {
        private readonly PlanetData _planet;

        public ReadOnlyPlanetData(PlanetData planet, long position)
        {
            Position = position;
            _planet = planet ?? throw new ArgumentNullException(nameof(planet));
        }

        public long Position { get; }
        public string Name => _planet.Name;
        public int ProductionRate => _planet.ProductionRate;
        public double Power => _planet.Power;
        public int Ships => _planet.Ships;
        public Guid? OwnerId => _planet.OwnerId;
    }
}