using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Presence.Events
{
    [Serializable]
    public class LeaderChanged : IClientEvent, IPresenceEvent
    {
        public LeaderChanged(string hubId, Guid? userId)
        {
            HubId = hubId;
            UserId = userId;
        }

        public string HubId { get; set; }
        public Guid? UserId { get; set; }
    }
}