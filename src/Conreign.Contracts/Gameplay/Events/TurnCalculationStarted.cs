using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnCalculationStarted : IClientEvent
    {
        public TurnCalculationStarted(int turn)
        {
            Turn = turn;
            Timestamp = DateTime.UtcNow;
        }

        public int Turn { get; }
        public DateTime Timestamp { get; }
    }
}