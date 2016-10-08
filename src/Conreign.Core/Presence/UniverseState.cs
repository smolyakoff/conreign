using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Presence
{
    public class UniverseState
    {
        public Dictionary<Guid, IConnectable> Connections { get; } = new Dictionary<Guid, IConnectable>();
    }
}