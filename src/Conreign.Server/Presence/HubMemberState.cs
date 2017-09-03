using System;
using System.Collections.Generic;

namespace Conreign.Server.Presence
{
    public class HubMemberState
    {
        public HashSet<Guid> ConnectionIds { get; set; } = new HashSet<Guid>();
    }
}