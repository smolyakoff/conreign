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
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public ReadOnlyPlanetData this[long position] => 
            new ReadOnlyPlanetData(_map[position], position);

        public ReadOnlyPlanetData this[int x, int y] => 
            new ReadOnlyPlanetData(_map[x, y], new Coordinate(x, y, Width, Height).Position);

        public int Height => _map.Height;
        public long MaxDistance => _map.MaxDistance;
        public int Width => _map.Width;

        public IEnumerator<ReadOnlyPlanetData> GetEnumerator()
        {
            return _map.Select(x => new ReadOnlyPlanetData(x.Value, x.Key)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.Select(x => new ReadOnlyPlanetData(x.Value, x.Key)).GetEnumerator();
        }

        public long CalculateDistance(long from, long to)
        {
            return _map.CalculateDistance(from, to);
        }
    }
}