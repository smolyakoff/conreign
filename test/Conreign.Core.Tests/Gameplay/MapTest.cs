using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Gameplay;
using Xunit;

namespace Conreign.Core.Tests.Gameplay
{
    public class MapTest
    {
        [Theory]
        [InlineData(6, 4, 1, 2, 3, 0)]
        [InlineData(4, 4, 1, 3, 0, 3)]
        [InlineData(3, 10, 2, 1, 2, 9)]
        public void CalculateRoute__Generates_Contiguous_Path(int width, int height, int fromX, int fromY, int toX,
            int toY)
        {
            var state = new MapData
            {
                Width = width,
                Height = height
            };
            var map = new Map(state)
            {
                [fromX, fromY] = new PlanetData {Name = "A"},
                [toX, toY] = new PlanetData {Name = "B"}
            };
            var route = map.GenerateRoute("A", "B");

            // Assert start and end points
            var source = new Coordinate(fromX, fromY, width, height);
            var destination = new Coordinate(toX, toY, width, height);
            Assert.Equal(source.Position, route[0]);
            Assert.Equal(destination.Position, route[route.Count - 1]);

            // Assert uniqueness
            Assert.Equal(new HashSet<long>(route).Count, route.Count);

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