using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Presence;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class GameData : IRoomData
    {
        public string RoomId { get; set; }
        public HashSet<Guid> DeadPlayers = new HashSet<Guid>();
        public int Turn { get; set; }
        public List<FleetData> WaitingFleets { get; set; } = new List<FleetData>();
        public List<MovingFleetData> MovingFleets { get; set; } = new List<MovingFleetData>();
        public TurnStatus TurnStatus { get; set; }
        public RoomMode Mode => RoomMode.Game;
        public Dictionary<Guid, TurnStatus> TurnStatuses { get; set; } = new Dictionary<Guid, TurnStatus>(0);
        public List<EventEnvelope> Events { get; set; } = new List<EventEnvelope>();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public Dictionary<Guid, PresenceStatus> PresenceStatuses { get; set; } = new Dictionary<Guid, PresenceStatus>();
        public MapData Map { get; set; } = new MapData();
        public Guid? LeaderUserId { get; set; }
    }
}