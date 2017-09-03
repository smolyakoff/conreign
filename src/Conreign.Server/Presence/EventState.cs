using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;

namespace Conreign.Server.Presence
{
    public class EventState
    {
        public HashSet<Guid> Recipients { get; set; }
        public IClientEvent Event { get; set; }
    }
}