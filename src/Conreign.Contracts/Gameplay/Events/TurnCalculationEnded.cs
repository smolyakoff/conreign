using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    public class TurnCalculationEnded : IClientEvent, IServerEvent
    {
        public TurnCalculationEnded(int turn, MapData map, List<MovingFleetData> movingFleets, bool isGameEnded)
        {
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
    }
}