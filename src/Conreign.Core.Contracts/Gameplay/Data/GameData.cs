using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class GameData
    {
        public GameData(MapData map, List<PlayerData> players, Dictionary<Guid, IClientObserver> hubMembers)
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
            Map = map;
            Players = players;
            HubMembers = hubMembers;
        }

        public MapData Map { get; }
        public List<PlayerData> Players { get; }
        public Dictionary<Guid, IClientObserver> HubMembers { get; }
    }
}
