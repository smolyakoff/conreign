using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Presence
{
    public class EventState
    {
        public HashSet<Guid> Recipients { get; set; }
        public IClientEvent Event { get; set; }
    }
}