using System;
using System.Collections.Generic;
using Conreign.Server.Contracts.Presence;

namespace Conreign.Server.Presence
{
    public class HubState
    {
        public Dictionary<Guid, HubMemberState> Members { get; set; } = new Dictionary<Guid, HubMemberState>();
        public List<Guid> JoinOrder { get; set; } = new List<Guid>();
        public List<EventState> Events { get; set; } = new List<EventState>();
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}