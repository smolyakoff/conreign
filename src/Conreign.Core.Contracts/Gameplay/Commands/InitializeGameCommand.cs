using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Commands
{
    [Serializable]
    [Immutable]
    public class InitializeGameCommand
    {
        public InitializeGameCommand(MapState map, List<PlayerOptionsState> players, Dictionary<Guid, IObserver> hubMembers)
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

        public MapState Map { get; }
        public List<PlayerOptionsState> Players { get; }
        public Dictionary<Guid, IObserver> HubMembers { get; }
    }
}
