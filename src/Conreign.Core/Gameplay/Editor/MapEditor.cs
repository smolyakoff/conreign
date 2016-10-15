using System;
using System.Collections.Generic;
using System.Linq;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Gameplay.Validators;
using Conreign.Core.Utility;

namespace Conreign.Core.Gameplay.Editor
{
    public class MapEditor
    {
        private readonly MapEditorState _state;

        private readonly IPlanetGenerator _playerPlanetGenerator;
        private readonly IPlanetGenerator _neutralPlanetGenerator;
        private readonly Map _map;
        private readonly IEnumerator<string> _namesEnumerator;

        public MapEditor(MapEditorState state, IPlanetGenerator playerPlanetGenerator, IPlanetGenerator neutralPlanetGenerator)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
            if (playerPlanetGenerator == null)
            {
                throw new ArgumentNullException(nameof(playerPlanetGenerator));
            }
            if (neutralPlanetGenerator == null)
            {
                throw new ArgumentNullException(nameof(neutralPlanetGenerator));
            }
            _playerPlanetGenerator = playerPlanetGenerator;
            _neutralPlanetGenerator = neutralPlanetGenerator;
            _state = state;
            _namesEnumerator = new ResetableEnumerator<string>(() => Sequences.PlanetNames);
            // Skip some names if map already has planets
            for (var i = 0; i < _state.Map.Planets.Count; i++)
            {
                _namesEnumerator.MoveNext();
            }
            _map = new Map(_state.Map);
        }

        public bool CanPlacePlanet => _map.FreeCellsCount > 0;
        public int MapWidth => _map.Width;
        public int MapHeigth => _map.Height;
        public int NeutralPlanetsCount => _state.NeutralPlanetsCount;

        public void Generate()
        {
            _namesEnumerator.Reset();
            _map.Reset();
            foreach (var pair in GeneratePlanets())
            {
                _map[pair.Key] = pair.Value;
            }
        }

        public void Generate(GameOptionsData options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.EnsureIsValid(new GameOptionsValidator(_state.NeutralPlanetsCount));
            _state.NeutralPlanetsCount = options.NeutralPlanetsCount;
            _map.Reset(options.MapWidth, options.MapHeight);
            Generate();
        }

        public void PlacePlanet(Guid playerId)
        {
            if (_state.Players.Contains(playerId))
            {
                return;
            }
            if (!CanPlacePlanet)
            {
                throw new InvalidOperationException("No free cells on the map");
            }
            var freeCoordinate = Sequences
                .NumbersRangeLong(0, _map.CellsCount)
                .Where(x => !_map.ContainsPlanet(x))
                .Shuffle()
                .First();
            _namesEnumerator.MoveNext();
            var planet = _playerPlanetGenerator.Generate(_namesEnumerator.Current, playerId);
            _map[freeCoordinate] = planet;
        }

        private IEnumerable<KeyValuePair<long, PlanetData>> GeneratePlanets()
        {
            var totalCells = _map.CellsCount;
            var totalPlanets = _state.NeutralPlanetsCount + _state.Players.Count;
            var planets = Sequences
                .NumbersRangeLong(0, totalCells)
                .Shuffle()
                .Take(totalPlanets)
                .Select(GeneratePlanet);
            return planets;
        }

        private KeyValuePair<long, PlanetData> GeneratePlanet(long coordinate, int index)
        {
            _namesEnumerator.MoveNext();
            var name = _namesEnumerator.Current;
            var planet = index < _state.Players.Count 
                ? _playerPlanetGenerator.Generate(name, _state.Players[index]) 
                : _neutralPlanetGenerator.Generate(name, null);
            return new KeyValuePair<long, PlanetData>(coordinate, planet);
        }
    }
}

