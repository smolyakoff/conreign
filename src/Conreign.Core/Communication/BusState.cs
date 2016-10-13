using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Communication
{
    public class BusState
    {
        public Guid StreamId { get; set; }
        public string Topic { get; set; }
        public Dictionary<IEventHandler, Guid> HandlerSubscriptions { get; set; } = new Dictionary<IEventHandler, Guid>();
    }
}