using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class MapUpdated : IClientEvent
    {
        public MapUpdated(MapData map)
        {
            Map = map;
        }

        public MapData Map { get; set; }
    }
}