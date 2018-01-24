using System;
using System.Collections.Generic;

namespace Conreign.Server.Presence
{
    public class HubMemberState
    {
        public HubMemberType Type { get; set; } = HubMemberType.Client;
        public HashSet<Guid> ConnectionIds { get; set; } = new HashSet<Guid>();
        public DateTime? ConnectionIdsChangedAt { get; set; }
    }
}