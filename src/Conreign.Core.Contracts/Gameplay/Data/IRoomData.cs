using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public interface IRoomData
    {
        RoomMode Mode { get; }
        List<IClientEvent> Events { get; }
        List<PlayerData> Players { get; }
        Dictionary<Guid, PresenceStatus> PlayerStatuses { get; }
        MapData Map { get; }
        Guid LeaderUserId { get; set; }
    }
}