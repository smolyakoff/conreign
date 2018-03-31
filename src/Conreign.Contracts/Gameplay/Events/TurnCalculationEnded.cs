using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    public class TurnCalculationEnded : IRoomEvent, IClientEvent, IServerEvent
    {
        public TurnCalculationEnded(string roomId, int turn, MapData map, List<MovingFleetData> movingFleets, bool isGameEnded)
        {
            RoomId = roomId;
            Turn = turn;
            Map = map;
            MovingFleets = movingFleets;
            IsGameEnded = isGameEnded;
            Timestamp = DateTime.UtcNow;
        }

        public bool IsGameEnded { get; }
        public int Turn { get; }
        public MapData Map { get; }
        public List<MovingFleetData> MovingFleets { get; }
        public DateTime Timestamp { get; }
        public string RoomId { get; }
    }
}