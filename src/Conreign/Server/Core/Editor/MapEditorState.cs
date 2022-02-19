using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Core.Editor;

public class MapEditorState
{
    public MapData Map { get; set; } = new();
    public int NeutralPlanetsCount { get; set; } = Defaults.NeutralPlayersCount;
    public List<Guid> Players { get; set; } = new();
}