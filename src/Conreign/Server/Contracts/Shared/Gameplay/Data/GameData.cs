using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Presence;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

[Serializable]
[Immutable]
public class GameData : IRoomData
{
    public HashSet<Guid> DeadPlayers = new();
    public int Turn { get; set; }
    public List<FleetData> WaitingFleets { get; set; } = new();
    public List<MovingFleetData> MovingFleets { get; set; } = new();
    public TurnStatus TurnStatus { get; set; }
    public Dictionary<Guid, TurnStatus> TurnStatuses { get; set; } = new(0);
    public string RoomId { get; set; }
    public RoomMode Mode => RoomMode.Game;
    public List<EventEnvelope> Events { get; set; } = new();
    public List<PlayerData> Players { get; set; } = new();
    public Dictionary<Guid, PresenceStatus> PresenceStatuses { get; set; } = new();
    public MapData Map { get; set; } = new();
    public Guid? LeaderUserId { get; set; }
}