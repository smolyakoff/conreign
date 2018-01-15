using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;
using Xunit;

namespace Conreign.Core.Tests.Gameplay
{
    public class MapTest
    {
        [Theory]
        [InlineData(6, 4, 1, 2, 3, 0)]
        [InlineData(4, 4, 1, 3, 0, 3)]
        [InlineData(4, 10, 2, 1, 2, 9)]
        public void CalculateRoute__Generates_Contiguous_Path(int width, int height, int fromX, int fromY, int toX,
            int toY)
        {
            var state = new MapData
            {
                Width = width,
                Height = height
            };
            var map = new Map(state);
            var source = new Coordinate(fromX, fromY, width, height);
            var destination = new Coordinate(toX, toY, width, height);
            var route = map.GenerateRoute(source.Position, destination.Position);

            // Assert start and end points
            Assert.Equal(source.Position, route[0]);
            Assert.Equal(destination.Position, route[route.Count - 1]);

            // Assert uniqueness
            Assert.Equal(new HashSet<int>(route).Count, route.Count);

            // Assert adjacency
            for (var i = 1; i < route.Count; i++)
            {
                var current = new Coordinate(route[i], width, height);
                var previous = new Coordinate(route[i - 1], width, height);
                var diff = current.X - previous.X + current.Y - previous.Y;
                Assert.Equal(1, Math.Abs(diff));
            }
        }
    }
}