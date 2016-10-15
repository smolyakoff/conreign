using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnCalculationEnded : IClientEvent
    {
        public TurnCalculationEnded(int turn, MapData map, List<MovingFleetData> movingFleets)
        {
            Turn = turn;
            Map = map;
            MovingFleets = movingFleets;
        }

        public int Turn { get; }
        public MapData Map { get; }
        public List<MovingFleetData> MovingFleets { get; }
    }
}