﻿using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Contracts.Gameplay.Data
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