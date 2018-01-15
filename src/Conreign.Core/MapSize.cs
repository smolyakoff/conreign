using System;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Core
{
    public struct MapSize
    {
        public MapSize(int width = DefaultWidth, int height = DefaultHeight)
        {
            if (width < Minimum || width > Maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(width), 
                    width, 
                    $"Expected map width to be from {Minimum} to {Maximum}. Got {width}.");
            }
            if (height < Minimum || height > Maximum)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(height),
                    height,
                    $"Expected map height to be from {Minimum} to {Maximum}. Got {height}.");
            }
            Width = width;
            Height = height;
        }

        public int Width { get; }
        public int Height { get; }
        public int CellsCount => Width * Height;
        public bool IsMaximum => Width == Maximum && Height == Maximum;
        public int MaximumDistance
        {
            get
            {
                var dim = Width < Height ? Width : Height;
                return 2 * dim - 2 + Math.Abs(Width - Height);
            }
        }

        public MapSize Increment()
        {
            if (IsMaximum)
            {
                throw new InvalidOperationException("Map size is already maximum, not possible to increment.");
            }
            var widthDelta = Width <= Height ? 1 : 0;
            var heightDelta = Height < Width ? 1 : 0;
            return new MapSize(Width + widthDelta, Height + heightDelta);
        }

        public override string ToString()
        {
            return $"W{Width}xH{Height}";
        }

        public const int MaximumCellsCount = Maximum * Maximum;
        private const int DefaultWidth = GameOptionsData.DefaultMapWidth;
        private const int DefaultHeight = GameOptionsData.DefaultMapHeight;
        private const int Minimum = GameOptionsData.MinumumMapSize;
        private const int Maximum = GameOptionsData.MaximumMapSize;
    }
}
