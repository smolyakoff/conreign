using System.Collections.Generic;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Presence
{
    public class UniverseState
    {
        public Dictionary<string, IDisconnectable> Connections { get; } = new Dictionary<string, IDisconnectable>();
    }
}