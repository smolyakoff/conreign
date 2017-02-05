using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class MapUpdated : IClientEvent, IRoomEvent
    {
        public MapUpdated(string roomId, MapData map)
        {
            RoomId = roomId;
            Map = map;
        }

        public string RoomId { get; }
        public MapData Map { get; }
    }
}