using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Presence;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    // TODO: Make it immutable, including inner data structures (Map, Players, etc...)
    public class GameStarted : IServerEvent, IClientEvent
    {
        public GameStarted(
            List<PlayerData> players, 
            Dictionary<Guid, PresenceStatus> presenceStatuses, 
            MapData map,
            Guid leaderUserId)
        {
            Players = players ?? throw new ArgumentNullException(nameof(players));
            PresenceStatuses = presenceStatuses ?? throw new ArgumentNullException(nameof(presenceStatuses));
            Map = map ?? throw new ArgumentNullException(nameof(map));
            LeaderUserId = leaderUserId;
        }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public List<PlayerData> Players { get; }
        public Dictionary<Guid, PresenceStatus> PresenceStatuses { get; }
        public MapData Map { get; }
        public Guid LeaderUserId { get; }
    }
}