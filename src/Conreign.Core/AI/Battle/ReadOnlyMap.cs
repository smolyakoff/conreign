using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Gameplay;

namespace Conreign.Core.AI.Battle
{
    public class ReadOnlyMap : IEnumerable<ReadOnlyPlanetData>
    {
        private readonly Map _map;

        public ReadOnlyMap(Map map)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            _map = map;
        }

        public ReadOnlyPlanetData this[string name] => new ReadOnlyPlanetData(_map[name]);

        public ReadOnlyPlanetData this[long coordinate] => new ReadOnlyPlanetData(_map[coordinate]);

        public ReadOnlyPlanetData this[int x, int y] => new ReadOnlyPlanetData(_map[x, y]);

        public int Height => _map.Height;
        public long MaxDistance => _map.MaxDistance;
        public int Width => _map.Width;

        public IEnumerator<ReadOnlyPlanetData> GetEnumerator()
        {
            return _map.Select(x => new ReadOnlyPlanetData(x)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.Select(x => new ReadOnlyPlanetData(x)).GetEnumerator();
        }

        public long CalculateDistance(string from, string to)
        {
            return _map.CalculateDistance(from, to);
        }

        public Coordinate GetPlanetCoordinateByName(string name)
        {
            return _map.GetPlanetCoordinateByName(name);
        }

        public long GetPlanetPositionByName(string name)
        {
            return _map.GetPlanetPositionByName(name);
        }
    }
}