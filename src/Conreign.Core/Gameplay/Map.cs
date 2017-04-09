using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class Map : IEnumerable<PlanetData>
    {
        private readonly Random _random = new Random();
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
        public long CellsCount => (long) _state.Width*_state.Height;

        public long MaxDistance
        {
            get
            {
                var dim = Width < Height ? Width : Height;
                return 2*dim - 2 + Math.Abs(Width - Height);
            }
        }

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
            set
            {
                if (value == null)
                {
                    _state.Planets.Remove(coordinate);
                    return;
                }
                _state.Planets[coordinate] = value;
            }
        }

        public PlanetData this[int x, int y]
        {
            get
            {
                EnsureCoordinateIsValid(x, y);
                var coordinate = new Coordinate(x, y, Width, Height);
                return this[coordinate.Position];
            }
            set
            {
                EnsureCoordinateIsValid(x, y);
                var coordinate = new Coordinate(x, y, Width, Height);
                this[coordinate.Position] = value;
            }
        }

        public PlanetData this[string name] => GetPlanetByName(name);

        public IEnumerator<PlanetData> GetEnumerator()
        {
            return _state.Planets.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _state.Planets.Values.GetEnumerator();
        }

        public bool ContainsPlanet(long coordinate)
        {
            return _state.Planets.ContainsKey(coordinate);
        }

        public Coordinate GetPlanetCoordinateByName(string name)
        {
            var position = GetPlanetPositionByName(name);
            return new Coordinate(position, Width, Height);
        }

        public long GetPlanetPositionByName(string name)
        {
            var positions = _state.Planets
                .Where(pair => pair.Value.Name == name)
                .Select(x => x.Key)
                .ToList();
            if (positions.Count == 0)
            {
                throw new InvalidOperationException($"Planet {name} was not found.");
            }
            var position = positions[0];
            return position;
        }

        public PlanetData GetPlanetByNameOrNull(string name)
        {
            return _state.Planets.Values.FirstOrDefault(x => x.Name == name);
        }

        public PlanetData GetPlanetByName(string name)
        {
            var planet = GetPlanetByNameOrNull(name);
            if (planet == null)
            {
                throw new InvalidOperationException($"Planet {name} was not found");
            }
            return planet;
            ;
        }

        public long CalculateDistance(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                throw new ArgumentException("From cannot be null or empty.", nameof(from));
            }
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("To cannot be null or empty.", nameof(to));
            }
            var source = GetPlanetCoordinateByName(from);
            var destination = GetPlanetCoordinateByName(to);
            var distanceX = Math.Abs(destination.X - source.X);
            var distanceY = Math.Abs(destination.Y - source.Y);
            return distanceX + distanceY + 1;
        }

        public List<long> GenerateRoute(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
            {
                throw new ArgumentException("From cannot be null or empty.", nameof(from));
            }
            if (string.IsNullOrEmpty(to))
            {
                throw new ArgumentException("To cannot be null or empty.", nameof(to));
            }
            var source = GetPlanetCoordinateByName(from);
            var destination = GetPlanetCoordinateByName(to);
            var distanceX = Math.Abs(destination.X - source.X);
            var distanceY = Math.Abs(destination.Y - source.Y);
            var currentPosition = source.Position;
            var destinationPosition = destination.Position;
            var path = new List<long>(distanceX + distanceY + 1) {currentPosition};
            var dx = destination.X >= source.X ? 1 : -1;
            var dy = destination.Y >= source.Y ? Width : -Width;
            (var delta, var nextDelta) = distanceX > distanceY ? (dx, dy) : (dy, dx);
            for (var i = 0; i < Math.Abs(distanceX - distanceY); i++)
            {
                currentPosition += delta;
                path.Add(currentPosition);
            }
            while (currentPosition != destinationPosition)
            {
                currentPosition += delta;
                path.Add(currentPosition);
                (delta, nextDelta) = (nextDelta, delta);
            }
            return path;
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

        private void EnsureCoordinateIsValid(int x, int y)
        {
            if (x < 0 || x >= Width)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Expected x to be from 0 to {Width - 1}. Got: {x}.");
            }
            if (y < 0 || y >= Height)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Expected y to be from 0 to {Height - 1}. Got: {y}.");
            }
        }

        private void EnsureCoordinateIsValid(long coordinate)
        {
            if (coordinate >= CellsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(coordinate),
                    $"Expected coordinate to be between 0 and {CellsCount - 1}.");
            }
        }
    }
}