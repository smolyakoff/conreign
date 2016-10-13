using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Presence
{
    public class HubState
    {
        public Dictionary<Guid, IPublisher<IEvent>> Members { get; set; } = new Dictionary<Guid, IPublisher<IEvent>>();
        public List<Guid> JoinOrder { get; set; } = new List<Guid>();
        public List<EventState> Events { get; set; } = new List<EventState>();
    }
}