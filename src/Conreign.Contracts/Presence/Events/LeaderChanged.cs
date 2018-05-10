using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Presence.Events
{
    [Serializable]
    [Immutable]
    public class LeaderChanged : IClientEvent
    {
        public LeaderChanged(Guid? userId)
        {
            UserId = userId;
        }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Guid? UserId { get; }
    }
}