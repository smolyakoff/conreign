using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Conreign.Server.Core.Editor.Validators;
using Conreign.Server.Core.Utility;

namespace Conreign.Server.Core.Editor;

public sealed class MapEditor
{
    private readonly Map _map;
    private readonly IEnumerator<string> _namesEnumerator;
    private readonly IPlanetGenerator _neutralPlanetGenerator;

    private readonly IPlanetGenerator _playerPlanetGenerator;
    private readonly MapEditorState _state;

    public MapEditor(MapEditorState state, IPlanetGenerator playerPlanetGenerator,
        IPlanetGenerator neutralPlanetGenerator)
    {
        _playerPlanetGenerator = playerPlanetGenerator ??
                                 throw new ArgumentNullException(nameof(playerPlanetGenerator));
        _neutralPlanetGenerator = neutralPlanetGenerator ??
                                  throw new ArgumentNullException(nameof(neutralPlanetGenerator));
        _state = state ?? throw new ArgumentNullException(nameof(state));
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

        options.EnsureIsValid(new GameOptionsValidator(_state.Players.Count));
        var currentOptions = new GameOptionsData
        {
            MapHeight = _state.Map.Height,
            MapWidth = _state.Map.Width,
            NeutralPlanetsCount = _state.NeutralPlanetsCount
        };
        if (currentOptions != options)
        {
            _state.NeutralPlanetsCount = options.NeutralPlanetsCount;
            _map.Reset(options.MapWidth, options.MapHeight);
        }

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

        _state.Players.Add(playerId);
        var freeCoordinate = Sequences
            .NumbersRangeLong(0, _map.CellsCount)
            .Where(x => !_map.ContainsPlanet(x))
            .Shuffle()
            .First();
        _namesEnumerator.MoveNext();
        var planet = _playerPlanetGenerator.Generate(_namesEnumerator.Current, playerId);
        _map[freeCoordinate] = planet;
    }

    private IEnumerable<KeyValuePair<int, PlanetData>> GeneratePlanets()
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

    private KeyValuePair<int, PlanetData> GeneratePlanet(int coordinate, int index)
    {
        _namesEnumerator.MoveNext();
        var name = _namesEnumerator.Current;
        var planet = index < _state.Players.Count
            ? _playerPlanetGenerator.Generate(name, _state.Players[index])
            : _neutralPlanetGenerator.Generate(name, null);
        return new KeyValuePair<int, PlanetData>(coordinate, planet);
    }
}