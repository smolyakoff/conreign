using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Immutable]
    [Serializable]
    public class GameTicked : IClientEvent
    {
        public GameTicked(int tick)
        {
            Tick = tick;
            Timestamp = DateTime.UtcNow;
        }

        public int Tick { get; }
        public DateTime Timestamp { get; }
    }
}