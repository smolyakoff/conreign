using System;
using Conreign.Contracts.Communication;

namespace Conreign.Contracts.Presence.Events
{
    [Serializable]
    public class LeaderChanged : IClientEvent, IPresenceEvent
    {
        public LeaderChanged(string hubId, Guid? userId)
        {
            HubId = hubId;
            UserId = userId;
        }

        public Guid? UserId { get; set; }
        public DateTime Timestamp { get; }

        public string HubId { get; set; }
    }
}