using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Presence.Events
{
    [Serializable]
    [Immutable]
    public class UserStatusChanged : IClientEvent, IPresenceEvent
    {
        public UserStatusChanged(string hubId, Guid userId, PresenceStatus status)
        {
            HubId = hubId;
            UserId = userId;
            Status = status;
            Timestamp = DateTime.UtcNow;
        }

        public PresenceStatus Status { get; }
        public Guid UserId { get; }
        public DateTime Timestamp { get; }
        public string HubId { get; }
    }
}