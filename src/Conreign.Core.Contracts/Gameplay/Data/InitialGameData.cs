using System;
using System.Collections.Generic;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class InitialGameData
    {
        public InitialGameData(
            Guid initiatorId,
            MapData map, 
            List<PlayerData> players,
            Dictionary<Guid, HashSet<Guid>> hubMembers,
            List<Guid> hubJoinOrder)
        {
            if (map == null)
            {
                throw new ArgumentNullException(nameof(map));
            }
            if (players == null)
            {
                throw new ArgumentNullException(nameof(players));
            }
            if (hubMembers == null)
            {
                throw new ArgumentNullException(nameof(hubMembers));
            }
            if (hubJoinOrder == null)
            {
                throw new ArgumentNullException(nameof(hubJoinOrder));
            }
            InitiatorId = initiatorId;
            Map = map;
            Players = players;
            HubMembers = hubMembers;
            HubJoinOrder = hubJoinOrder;
        }

        public Guid InitiatorId { get; }
        public MapData Map { get; }
        public List<PlayerData> Players { get; }
        public Dictionary<Guid, HashSet<Guid>> HubMembers { get; }
        public List<Guid> HubJoinOrder { get; }
    }
}
