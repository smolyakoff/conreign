using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Presence;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class GameData : IRoomData
    {
        public RoomMode Mode => RoomMode.Game;
        public List<IClientEvent> Events { get; } = new List<IClientEvent>();
        public List<PlayerData> Players { get; } = new List<PlayerData>();
        public Dictionary<Guid, PresenceStatus> PlayerStatuses { get; } = new Dictionary<Guid, PresenceStatus>();
        public MapData Map { get; } = new MapData();
        public Guid? LeaderUserId { get; set; }
    }
}