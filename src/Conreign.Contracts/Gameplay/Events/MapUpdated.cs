using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    public class MapUpdated : IClientEvent
    {
        public MapUpdated(MapData map)
        {
            Map = map;
            Timestamp = DateTime.UtcNow;
        }

        public MapData Map { get; }
        public DateTime Timestamp { get; }
    }
}