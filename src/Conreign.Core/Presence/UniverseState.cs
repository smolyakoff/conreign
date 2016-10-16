using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;

namespace Conreign.Core.Presence
{
    public class UniverseState
    {
        public Dictionary<Guid, IPublisher<Disconnected>> Connections { get; set; } = new Dictionary<Guid, IPublisher<Disconnected>>();
    }
}