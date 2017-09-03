using System;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Data
{
    [Serializable]
    [Immutable]
    public class FleetCancelationData
    {
        public int Index { get; set; }
    }
}