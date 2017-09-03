using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class MapUpdated : IClientEvent, IRoomEvent
    {
        public MapUpdated(string roomId, MapData map)
        {
            RoomId = roomId;
            Map = map;
            Timestamp = DateTime.UtcNow;
        }

        public MapData Map { get; }
        public DateTime Timestamp { get; }

        public string RoomId { get; }
    }
}