using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Server.Contracts.Presence;

namespace Conreign.Server.Contracts.Gameplay
{
    [Serializable]
    public class InitialGameData
    {
        public InitialGameData(
            Guid initiatorUserId,
            MapData map,
            List<PlayerData> players,
            Dictionary<Guid, HubMemberState> hubMembers,
            List<Guid> hubJoinOrder)
        {
            InitiatorUserId = initiatorUserId;
            Map = map ?? throw new ArgumentNullException(nameof(map));
            Players = players ?? throw new ArgumentNullException(nameof(players));
            HubMembers = hubMembers ?? throw new ArgumentNullException(nameof(hubMembers));
            HubJoinOrder = hubJoinOrder ?? throw new ArgumentNullException(nameof(hubJoinOrder));
        }

        public Guid InitiatorUserId { get; }
        public MapData Map { get; }
        public List<PlayerData> Players { get; }
        public Dictionary<Guid, HubMemberState> HubMembers { get; }
        public List<Guid> HubJoinOrder { get; }
    }
}