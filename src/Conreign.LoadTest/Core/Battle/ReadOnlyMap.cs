using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core;

namespace Conreign.LoadTest.Core.Battle
{
    public class ReadOnlyMap : IEnumerable<ReadOnlyPlanetData>
    {
        private readonly Map _map;

        public ReadOnlyMap(Map map)
        {
            _map = map ?? throw new ArgumentNullException(nameof(map));
        }

        public ReadOnlyPlanetData this[int position] =>
            new ReadOnlyPlanetData(_map[position], position);

        public ReadOnlyPlanetData this[int x, int y] =>
            new ReadOnlyPlanetData(_map[x, y], new Coordinate(x, y, Width, Height).Position);

        public int Height => _map.Height;
        public int MaxDistance => _map.MaximumDistance;
        public int Width => _map.Width;

        public IEnumerator<ReadOnlyPlanetData> GetEnumerator()
        {
            return _map.Select(x => new ReadOnlyPlanetData(x.Value, x.Key)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _map.Select(x => new ReadOnlyPlanetData(x.Value, x.Key)).GetEnumerator();
        }

        public int CalculateDistance(int from, int to)
        {
            return _map.CalculateDistance(from, to);
        }
    }
}