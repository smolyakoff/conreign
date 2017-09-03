using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Presence;

namespace Conreign.Contracts.Gameplay.Data
{
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
}