using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Presence.Events
{
    [Serializable]
    public class UserStatusChanged : IClientEvent
    {
        public PresenceStatus Status { get; set; }
        public Guid UserId { get; set; }
    }
}
