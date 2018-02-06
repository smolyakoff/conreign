using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Presence.Events
{
    [Serializable]
    [Immutable]
    public class LeaderChanged : IClientEvent, IPresenceEvent
    {
        public LeaderChanged(string hubId, Guid? userId)
        {
            HubId = hubId;
            UserId = userId;
        }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public Guid? UserId { get; }
        public string HubId { get; }
    }
}