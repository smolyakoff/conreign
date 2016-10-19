using System;
using System.Collections.Generic;

namespace Conreign.Core.Presence
{
    public class HubState
    {
        public Dictionary<Guid, HubMemberState> Members { get; set; } = new Dictionary<Guid, HubMemberState>();
        public List<Guid> JoinOrder { get; set; } = new List<Guid>();
        public List<EventState> Events { get; set; } = new List<EventState>();
    }
}