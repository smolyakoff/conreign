using System;
using System.Collections.Generic;

namespace Conreign.Server.Contracts.Presence
{
    public class HubMemberState
    {
        public HashSet<Guid> ConnectionIds { get; set; } = new HashSet<Guid>();
        public DateTime? ConnectionIdsChangedAt { get; set; }
    }
}