using System;
using System.Collections;
using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class Map : IEnumerable<KeyValuePair<int, PlanetData>>
    {
        private readonly MapData _state;

        public Map(MapData state)
        {
            _state = state ?? throw new ArgumentNullException(nameof(state));
        }

        public int Width => _state.Width;
        public int Height => _state.Height;
        public int FreeCellsCount => CellsCount - _state.Planets.Count;
        public int CellsCount => _state.Width*_state.Height;

        public int MaxDistance
        {
            get
            {
                var dim = Width < Height ? Width : Height;
                return 2*dim - 2 + Math.Abs(Width - Height);
            }
        }

        public IEnumerable<PlanetData> Planets => _state.Planets.Values;

        public PlanetData this[int position]
        {
            get
            {
                EnsurePositionIsValid(position);
                if (!ContainsPlanet(position))
                {
                    throw new ArgumentException($"There is no planet at {position}", nameof(position));
                }
                return _state.Planets[position];
            }
            set
            {
                if (value == null)
                {
                    _state.Planets.Remove(position);
                    return;
                }
                _state.Planets[position] = value;
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

        public bool ContainsPlanet(int position)
        {
            return _state.Planets.ContainsKey(position);
        }

        public int CalculateDistance(int from, int to)
        {
            EnsurePositionIsValid(from);
            EnsurePositionIsValid(to);

            var source = new Coordinate(from, Width, Height);
            var destination = new Coordinate(to, Width, Height);
            var distanceX = Math.Abs(destination.X - source.X);
            var distanceY = Math.Abs(destination.Y - source.Y);
            return distanceX + distanceY + 1;
        }

        public List<int> GenerateRoute(int from, int to)
        {
            EnsurePositionIsValid(from);
            EnsurePositionIsValid(to);

            var source = new Coordinate(from, Width, Height);
            var destination = new Coordinate(to, Width, Height);
            var distanceX = Math.Abs(destination.X - source.X);
            var distanceY = Math.Abs(destination.Y - source.Y);
            var currentPosition = source.Position;
            var destinationPosition = destination.Position;
            var path = new List<int>(distanceX + distanceY + 1) {currentPosition};
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

        public IEnumerator<KeyValuePair<int, PlanetData>> GetEnumerator()
        {
            return _state.Planets.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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

        private void EnsurePositionIsValid(int coordinate)
        {
            if (coordinate < 0 || coordinate >= CellsCount)
            {
                throw new ArgumentOutOfRangeException(nameof(coordinate),
                    $"Expected position to be between 0 and {CellsCount - 1}.");
            }
        }
    }
}