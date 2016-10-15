using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnCalculationStarted : IClientEvent
    {
        public TurnCalculationStarted(int turn)
        {
            Turn = turn;
        }

        public int Turn { get; }
    }
}