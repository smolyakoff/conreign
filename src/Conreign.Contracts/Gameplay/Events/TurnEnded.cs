using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnEnded : IClientEvent
    {
        public TurnEnded(Guid userId)
        {
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        public Guid UserId { get; }
        public DateTime Timestamp { get; }
    }
}