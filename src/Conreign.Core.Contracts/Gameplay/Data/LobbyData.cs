﻿using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    public class LobbyData : IRoomData
    {
        public string RoomId { get; set; }
        public RoomMode Mode => RoomMode.Lobby;
        public List<EventEnvelope> Events { get; set; } = new List<EventEnvelope>();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public Dictionary<Guid, PresenceStatus> PlayerStatuses { get; set; } = new Dictionary<Guid, PresenceStatus>();
        public MapData Map { get; set; } = new MapData();
        public Guid? LeaderUserId { get; set; }
    }
}