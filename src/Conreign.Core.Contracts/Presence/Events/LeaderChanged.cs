using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Presence.Events
{
    [Serializable]
    public class LeaderChanged : IClientEvent
    {
        public Guid? UserId { get; set; }
    }
}
