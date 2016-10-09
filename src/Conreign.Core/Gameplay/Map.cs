using System;
using System.Linq;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class Map
    {
        private readonly MapData _state;

        public Map(MapData state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _state = state;
        }

        public int Width => _state.Width;
        public int Height => _state.Height;
        public long FreeCellsCount => CellsCount - _state.Planets.Count;
        public long CellsCount => (long)_state.Width*_state.Height;

        public PlanetData this[long coordinate]
        {
            get
            {
                EnsureCoordinateIsValid(coordinate);
                if (!ContainsPlanet(coordinate))
                {
                    throw new ArgumentException($"There is no planet at {coordinate}", nameof(coordinate));
                }
                return _state.Planets[coordinate];
            }
            set { _state.Planets[coordinate] = value; }
        }

        public bool ContainsPlanet(long coordinate)
        {
            return _state.Planets.ContainsKey(coordinate);
        }

        public PlanetData GetPlanetByName(string name)
        {
            return _state.Planets.Values.FirstOrDefault(x => x.Name == name);
        }

        public void Reset()
        {
            _state.Planets.Clear();
        }

        public void Reset(int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width should be 1 or greater");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height should be 1 or greater");
            }
            Reset();
            _state.Width = width;
            _state.Height = height;
        }

        private void EnsureCoordinateIsValid(long coordinate)
        {
            if (coordinate >= CellsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(coordinate), $"Expected coordinate to be less than {CellsCount}");
            }
        }
    }
}
