using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Presence;

namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

public interface IRoomData
{
    string RoomId { get; }
    RoomMode Mode { get; }
    List<EventEnvelope> Events { get; }
    List<PlayerData> Players { get; }
    Dictionary<Guid, PresenceStatus> PresenceStatuses { get; }
    MapData Map { get; }
    Guid? LeaderUserId { get; set; }
}