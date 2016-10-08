using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Presence
{
    public class HubState
    {
        public Dictionary<Guid, IClientObserver> Members { get; set; } = new Dictionary<Guid, IClientObserver>();
        public List<Guid> JoinOrder { get; set; } = new List<Guid>();
        public List<EventState> Events { get; set; } = new List<EventState>();
    }
}