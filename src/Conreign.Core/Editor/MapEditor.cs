using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Utility;

namespace Conreign.Core.Editor
{
    public sealed class MapEditor
    {
        private readonly Map _map;
        private readonly IEnumerator<string> _namesEnumerator;
        private readonly IPlanetGenerator _neutralPlanetGenerator;
        private readonly IPlanetGenerator _playerPlanetGenerator;

        public MapEditor(MapData state,
            IPlanetGenerator playerPlanetGenerator,
            IPlanetGenerator neutralPlanetGenerator)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            _playerPlanetGenerator = playerPlanetGenerator ??
                                     throw new ArgumentNullException(nameof(playerPlanetGenerator));
            _neutralPlanetGenerator = neutralPlanetGenerator ??
                                      throw new ArgumentNullException(nameof(neutralPlanetGenerator));
            _namesEnumerator = new ResetableEnumerator<string>(() => Sequences.PlanetNames);
            // Skip some names if map already has planets
            for (var i = 0; i < state.Planets.Count; i++)
            {
                _namesEnumerator.MoveNext();
            }
            _map = new Map(state);
        }

        public void GenerateMap(int neutralPlanetsCount)
        {
            GenerateMap(_map.Size, _map.PlayerIds.ToHashSet(), neutralPlanetsCount);
        }

        public void GenerateMap(MapSize mapSize, HashSet<Guid> playerIds, int neutralPlanetsCount)
        {
            if (playerIds == null)
            {
                throw new ArgumentNullException(nameof(playerIds));
            }
            if (neutralPlanetsCount < 0)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(neutralPlanetsCount), 
                    "Neutral planets count should be 0 or greater.");
            }
            var planetsCount = playerIds.Count + neutralPlanetsCount;
            if (planetsCount > mapSize.CellsCount)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(mapSize), 
                    $"Map size {mapSize} is too small for {planetsCount} planets.");
            }
            _map.ClearAndResize(mapSize);
            _namesEnumerator.Reset();
            foreach (var pair in GeneratePlanets(playerIds.ToList(), neutralPlanetsCount))
            {
                _map[pair.Key] = pair.Value;
            }
        }

        public bool TryPlacePlanet(Guid playerId)
        {
            if (_map.ContainsPlanetOwnedBy(playerId) || _map.HasMaximumSize)
            {
                return false;
            }
            if (_map.FreeCellsCount == 0)
            {
                var newSize = _map.Size.Increment();
                GenerateMap(newSize, _map.PlayerIds.ToHashSet(), _map.NeutralPlanetsCount);
            }
            var freeCoordinate = Enumerable
                .Range(0, _map.CellsCount)
                .Shuffle()
                .First(x => !_map.ContainsPlanetAtPosition(x));
            _namesEnumerator.MoveNext();
            var planet = _playerPlanetGenerator.Generate(_namesEnumerator.Current, playerId);
            _map[freeCoordinate] = planet;
            return true;
        }

        private IEnumerable<KeyValuePair<int, PlanetData>> GeneratePlanets(IReadOnlyList<Guid> playerIds, int neutralPlanetsCount)
        {
            KeyValuePair<int, PlanetData> GeneratePlanet(int position, int index)
            {
                _namesEnumerator.MoveNext();
                var name = _namesEnumerator.Current;
                var planet = index < playerIds.Count
                    ? _playerPlanetGenerator.Generate(name, playerIds[index])
                    : _neutralPlanetGenerator.Generate(name, null);
                return new KeyValuePair<int, PlanetData>(position, planet);
            }
            var totalCells = _map.CellsCount;
            var totalPlanets = neutralPlanetsCount + playerIds.Count;
            var planets = Enumerable.Range(0, totalCells)
                .Shuffle()
                .Take(totalPlanets)
                .Select(GeneratePlanet);
            return planets;
        }

        
    }
}