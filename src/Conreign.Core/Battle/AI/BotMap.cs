using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Core.Battle.AI
{
    // This class assumes that planet positions never change
    public class BotMap : IReadOnlyMap
    {
        private readonly Map _map;
        private readonly Dictionary<string, int> _positionsByPlanetName;

        public BotMap(MapData state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _map = new Map(state);
            _positionsByPlanetName = new Dictionary<string, int>();
            
            foreach (var kv in _map)
            {
                _positionsByPlanetName[kv.Value.Name] = kv.Key;
            }
        }

        public int MaxDistance => _map.MaximumDistance;

        public IEnumerator<IPlanetData> GetEnumerator()
        {
            return _map.Planets.Cast<IPlanetData>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void UpdatePlanets(IEnumerable<PlanetData> planets)
        {
            foreach (var planet in planets)
            {
                var position = GetPosition(planet);
                _map[position] = planet;
            }
        }

        public int GetPosition(IPlanetData planet)
        {
            if (planet == null)
            {
                throw new ArgumentNullException(nameof(planet));
            }
            var exists = _positionsByPlanetName.TryGetValue(planet.Name, out int position);
            if (!exists)
            {
                throw new InvalidOperationException($"Planet '${planet.Name}' was not found.");
            }
            return position;
        }

        public int CalculateDistance(IPlanetData @from, IPlanetData to)
        {
            if (@from == null)
            {
                throw new ArgumentNullException(nameof(@from));
            }
            if (to == null)
            {
                throw new ArgumentNullException(nameof(to));
            }
            var fromPosition = GetPosition(@from);
            var toPosition = GetPosition(to);
            return _map.CalculateDistance(fromPosition, toPosition);
        }
    }
}