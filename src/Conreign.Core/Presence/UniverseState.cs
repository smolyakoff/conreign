using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Presence
{
    public class UniverseState
    {
        public Dictionary<Guid, IPublisher<ISystemEvent>> Connections { get; set; } = new Dictionary<Guid, IPublisher<ISystemEvent>>();
    }
}