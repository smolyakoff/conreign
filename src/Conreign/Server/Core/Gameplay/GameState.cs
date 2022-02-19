using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Conreign.Server.Core.Presence;

namespace Conreign.Server.Core.Gameplay;

public class GameState
{
    public string RoomId { get; set; }
    public bool IsEnded { get; set; }
    public bool IsStarted { get; set; }
    public int Turn { get; set; }
    public MapData Map { get; set; } = new();
    public List<PlayerData> Players { get; set; } = new();
    public HubState Hub { get; set; } = new();
    public Dictionary<Guid, PlayerGameState> PlayerStates { get; set; } = new();
}