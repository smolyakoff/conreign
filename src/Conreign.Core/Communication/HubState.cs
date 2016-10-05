using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Communication
{
    public class HubState
    {
        public Dictionary<Guid, IObserver> Members { get; set; } = new Dictionary<Guid, IObserver>();
        public List<EventState> Events { get; set; } = new List<EventState>();
    }
}