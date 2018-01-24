using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnCalculationEnded : IRoomEvent, IClientEvent, IServerEvent
    {
        public TurnCalculationEnded(string roomId, int turn, MapData map, List<MovingFleetData> movingFleets)
        {
            RoomId = roomId;
            Turn = turn;
            Map = map;
            MovingFleets = movingFleets;
            Timestamp = DateTime.UtcNow;
        }

        public int Turn { get; }
        public MapData Map { get; }
        public List<MovingFleetData> MovingFleets { get; }
        public DateTime Timestamp { get; }
        public string RoomId { get; }
    }
}