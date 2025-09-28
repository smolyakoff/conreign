using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Presence;

namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

[Serializable]
public class LobbyData : IRoomData
{
    public string RoomId { get; set; }
    public RoomMode Mode => RoomMode.Lobby;
    public List<EventEnvelope> Events { get; set; } = new();
    public List<PlayerData> Players { get; set; } = new();
    public Dictionary<Guid, PresenceStatus> PresenceStatuses { get; set; } = new();
    public MapData Map { get; set; } = new();
    public Guid? LeaderUserId { get; set; }
}