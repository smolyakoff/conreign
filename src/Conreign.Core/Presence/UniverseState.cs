using System;
using System.Collections.Generic;

namespace Conreign.Core.Presence
{
    public class UniverseState
    {
        public Dictionary<Guid, string> Connections { get; set; } = new Dictionary<Guid, string>();
    }
}