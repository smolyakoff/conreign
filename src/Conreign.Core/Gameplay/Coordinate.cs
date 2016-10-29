using System;

namespace Conreign.Core.Gameplay
{
    public struct Coordinate : IEquatable<Coordinate>
    {
        public Coordinate(long position, int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), $"Width should be 1 or greater.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height should be 1 or greater.");
            }
            var maxPosition = width*height - 1;
            if (position < 0 || position > maxPosition)
            {
                throw new ArgumentOutOfRangeException(nameof(position),
                    $"Expected position to be between 0 and {maxPosition}. Got: {position}.");
            }
            Position = position;
            Width = width;
            Height = height;
        }

        public Coordinate(int x, int y, int width, int height)
        {
            if (width <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(width), "Width should be 1 or greater.");
            }
            if (height <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(height), "Height should be 1 or greater.");
            }
            if (x < 0 || x >= width)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Expected x to be from 0 to {width - 1}.");
            }
            if (y < 0 || y >= height)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Expected y to be from 0 to {height - 1}.");
            }
            Width = width;
            Height = height;
            Position = x + y*width;
        }

        public long Position { get; }
        public int Width { get; }
        public int Height { get; }
        public int X => (int) (Position%Width);
        public int Y => (int) (Position/Width);

        public Coordinate Move(int x, int y)
        {
            var minX = -X;
            var maxX = Width - x - 1;
            if (x < minX || x > maxX)
            {
                throw new ArgumentOutOfRangeException(nameof(x), $"Expected x to be from {minX} to {maxX}. Got: {x}.");
            }
            var minY = -Y;
            var maxY = Height - y - 1;
            if (y < minY || y > maxY)
            {
                throw new ArgumentOutOfRangeException(nameof(y), $"Expected y to be from {minY} to {maxY}. Got: {y}.");
            }
            var resultX = X + x;
            var resultY = Y + y;
            return new Coordinate(resultX, resultY, Width, Height);
        }

        public bool Equals(Coordinate other)
        {
            return Position == other.Position && Width == other.Width && Height == other.Height;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            return obj is Coordinate && Equals((Coordinate) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = Position.GetHashCode();
                hashCode = (hashCode*397) ^ Width;
                hashCode = (hashCode*397) ^ Height;
                return hashCode;
            }
        }

        public static bool operator ==(Coordinate left, Coordinate right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Coordinate left, Coordinate right)
        {
            return !left.Equals(right);
        }
    }
}