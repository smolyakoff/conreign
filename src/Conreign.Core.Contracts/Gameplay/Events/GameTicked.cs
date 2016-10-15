using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Immutable]
    [Serializable]
    public class GameTicked : IClientEvent
    {
        public int Tick { get; }

        public GameTicked(int tick)
        {
            Tick = tick;
        }
    }
}
