using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Presence.Events
{
    [Serializable]
    [Immutable]
    public class UserStatusChanged : IClientEvent
    {
        public UserStatusChanged(Guid userId, PresenceStatus status)
        {
            UserId = userId;
            Status = status;
            Timestamp = DateTime.UtcNow;
        }

        public PresenceStatus Status { get; }
        public Guid UserId { get; }
        public DateTime Timestamp { get; }
    }
}