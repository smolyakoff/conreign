using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Presence.Events
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

        public PresenceStatus Status { get;  }
        public Guid UserId { get;  }
        public string HubId { get; }
        public DateTime Timestamp { get; }
    }
}