using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Presence;

namespace Conreign.Contracts.Gameplay.Data
{
    [Serializable]
    public class LobbyData : IRoomData
    {
        public string RoomId { get; set; }
        public RoomMode Mode => RoomMode.Lobby;
        public List<EventEnvelope> Events { get; set; } = new List<EventEnvelope>();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public Dictionary<Guid, PresenceStatus> PresenceStatuses { get; set; } = new Dictionary<Guid, PresenceStatus>();
        public MapData Map { get; set; } = new MapData();
        public Guid? LeaderUserId { get; set; }
        public int MinimumMapSize => GameOptionsData.MinumumMapSize;
        public int MaximumMapSize => GameOptionsData.MaximumMapSize;
        public int MaximumBotsCount => GameOptionsData.MaximumBotsCount;
    }
}