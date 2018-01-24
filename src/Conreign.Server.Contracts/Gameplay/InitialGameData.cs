using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Gameplay
{
    [Serializable]
    [Immutable]
    public class InitialGameData
    {
        public InitialGameData(
            Guid initiatorId,
            MapData map,
            List<PlayerData> players,
            Dictionary<Guid, HashSet<Guid>> clientConnections,
            List<Guid> hubJoinOrder)
        {
            InitiatorId = initiatorId;
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Players = players ?? throw new ArgumentNullException(nameof(players));
            ClientConnectionIds = clientConnections ?? throw new ArgumentNullException(nameof(clientConnections));
            HubJoinOrder = hubJoinOrder ?? throw new ArgumentNullException(nameof(hubJoinOrder));
        }

        public Guid InitiatorId { get; }
        public MapData Map { get; }
        public List<PlayerData> Players { get; }
        public Dictionary<Guid, HashSet<Guid>> ClientConnectionIds { get; }
        public List<Guid> HubJoinOrder { get; }
    }
}