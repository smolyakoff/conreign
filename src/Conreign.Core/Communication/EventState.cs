using System;
using System.Collections.Generic;

namespace Conreign.Core.Communication
{
    public class EventState
    {
        public HashSet<Guid> Recipients { get; set; }
        public object Event { get; set; }
    }
}