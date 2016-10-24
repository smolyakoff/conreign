using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class FleetCancelationData
    {
        public int Index { get; set; }
    }
}
