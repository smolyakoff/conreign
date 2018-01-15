using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Editor;
using Conreign.Core.Utility;
using Moq;
using Xunit;

namespace Conreign.Core.Tests.Editor
{
    public class MapEditorTest
    {
        private static Mock<IPlanetGenerator> CreatePlayerPlanetGeneratorMock()
        {
            var playerPlanetGeneratorMock = new Mock<IPlanetGenerator>();
            playerPlanetGeneratorMock
                .Setup(x => x.Generate(It.IsAny<string>(), It.IsAny<Guid?>()))
                .Returns((string name, Guid? ownerId) => new PlanetData {Name = name, OwnerId = ownerId});
            return playerPlanetGeneratorMock;
        }

        private static Mock<IPlanetGenerator> CreateNeutralPlanetGeneratorMock()
        {
            var neutralPlanetGeneratorMock = new Mock<IPlanetGenerator>();
            neutralPlanetGeneratorMock
                .Setup(x => x.Generate(It.IsAny<string>(), null))
                .Returns((string name, Guid? _) => new PlanetData {Name = name});
            return neutralPlanetGeneratorMock;
        }

        [Fact]
        public void GenerateMap__Randomly_Places_Requested_Number_Of_Planets()
        {
            var state = new MapData();
            var neutralPlanetGeneratorMock = CreateNeutralPlanetGeneratorMock();
            var playerPlanetGeneratorMock = CreatePlayerPlanetGeneratorMock();
            var mapEditor = new MapEditor(
                state,
                playerPlanetGeneratorMock.Object,
                neutralPlanetGeneratorMock.Object);
            var mapSize = new MapSize(4, 4);
            const int playerPlanetsCount = 3;
            var playerIds = Enumerable.Range(0, playerPlanetsCount).Select(_ => Guid.NewGuid()).ToHashSet();
            const int neutralPlanetsCount = 2;
            const int totalPlanetsCount = playerPlanetsCount + neutralPlanetsCount;
            mapEditor.GenerateMap(mapSize, playerIds, neutralPlanetsCount);


            Assert.Equal(mapSize.Width, state.Width);
            Assert.Equal(mapSize.Height, state.Height);
            Assert.Equal(neutralPlanetsCount, state.Planets.Values.Count(x => x.OwnerId == null));
            Assert.Equal(playerPlanetsCount, state.Planets.Values.Count(x => x.OwnerId != null));
            Assert.All(state.Planets, kv =>
            {
                var position = kv.Key;
                var planet = kv.Value;
                Assert.InRange(position, 0, mapSize.Width * mapSize.Height);
                Assert.NotNull(planet.Name);
            });
            Assert.Equal(
                Sequences.PlanetNames.Take(totalPlanetsCount),
                state.Planets.Values.Select(x => x.Name).OrderBy(x => x));
            neutralPlanetGeneratorMock.Verify(
                x => x.Generate(It.IsAny<string>(), null),
                Times.Exactly(2));
            playerPlanetGeneratorMock.Verify(
                x => x.Generate(
                    It.IsAny<string>(),
                    It.Is((Guid? ownerId) => ownerId != null && playerIds.Contains(ownerId.Value))),
                Times.Exactly(playerPlanetsCount));
        }

        [Fact]
        public void GenerateMap__Resets_State_And_Planet_Names_Sequence()
        {
            var initialPlanets = new Dictionary<int, PlanetData>
            {
                [1] = new PlanetData {Name = Sequences.PlanetNames.ElementAt(0), OwnerId = Guid.NewGuid()},
                [5] = new PlanetData {Name = Sequences.PlanetNames.ElementAt(1)}
            };
            var state = new MapData
            {
                Width = 4,
                Height = 4,
                Planets = initialPlanets.ToDictionary(x => x.Key, x => x.Value)
            };
            var neutralPlanetGeneratorMock = CreateNeutralPlanetGeneratorMock();
            var playerPlanetGeneratorMock = CreatePlayerPlanetGeneratorMock();
            var mapEditor = new MapEditor(
                state,
                playerPlanetGeneratorMock.Object,
                neutralPlanetGeneratorMock.Object);
            var mapSize = new MapSize(5, 5);
            const int playerPlanetsCount = 5;
            const int neutralPlanetsCount = 1;
            const int totalPlanetsCount = playerPlanetsCount + neutralPlanetsCount;
            var playerIds = Enumerable.Range(0, playerPlanetsCount).Select(_ => Guid.NewGuid()).ToHashSet();
            
            mapEditor.GenerateMap(mapSize, playerIds, neutralPlanetsCount);

            Assert.Equal(mapSize.Width, state.Width);
            Assert.Equal(mapSize.Height, state.Height);
            Assert.All(state.Planets.Values, planet => Assert.DoesNotContain(planet, initialPlanets.Values));
            Assert.Equal(
                Sequences.PlanetNames.Take(totalPlanetsCount),
                state.Planets.Values.Select(x => x.Name).OrderBy(x => x));
        }

        [Fact]
        public void TryPlacePlanet__Places_Planet_At_A_Random_Free_Cell()
        {
            var state = new MapData
            {
                Width = 4,
                Height = 4,
                Planets = new Dictionary<int, PlanetData>
                {
                    [2] = new PlanetData { Name = Sequences.PlanetNames.First() },
                    [5] = new PlanetData { Name = Sequences.PlanetNames.ElementAt(1), OwnerId = Guid.NewGuid() }
                }
            };
            var occupiedPositions = state.Planets.Keys.ToHashSet();
            var neutralPlanetGeneratorMock = CreateNeutralPlanetGeneratorMock();
            var playerPlanetGeneratorMock = CreatePlayerPlanetGeneratorMock();
            var mapEditor = new MapEditor(
                state,
                playerPlanetGeneratorMock.Object,
                neutralPlanetGeneratorMock.Object);
            var newPlayerId = Guid.NewGuid();
            var isPlaced = mapEditor.TryPlacePlanet(newPlayerId);

            Assert.True(isPlaced);
            Assert.Contains(state.Planets.Values, x => x.OwnerId == newPlayerId);
            var kv = state.Planets.FirstOrDefault(x => x.Value.OwnerId == newPlayerId);
            var position = kv.Key;
            var planet = kv.Value;
            Assert.DoesNotContain(position, occupiedPositions);
            Assert.Equal(planet.Name, Sequences.PlanetNames.ElementAt(occupiedPositions.Count));
            neutralPlanetGeneratorMock.Verify(x => x.Generate(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
            playerPlanetGeneratorMock.Verify(x => x.Generate(planet.Name, newPlayerId), Times.Exactly(1));
        }
    }
}