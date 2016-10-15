using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class InitialGameData
    {
        public InitialGameData(
            MapData map, 
            List<PlayerData> players,
            IPublisher<IServerEvent> hub,
            Dictionary<Guid, IPublisher<IEvent>> hubMembers, 
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
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }
            if (hubMembers == null)
            {
                throw new ArgumentNullException(nameof(hubMembers));
            }
            Map = map;
            Players = players;
            HubMembers = hubMembers;
            HubJoinOrder = hubJoinOrder;
            Hub = hub;
        }

        public MapData Map { get; }
        public List<PlayerData> Players { get; }
        public IPublisher<IServerEvent> Hub { get; }
        public Dictionary<Guid, IPublisher<IEvent>> HubMembers { get; }
        public List<Guid> HubJoinOrder { get; }
    }
}
