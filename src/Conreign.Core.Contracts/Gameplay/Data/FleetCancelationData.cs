using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    [Immutable]
    [Serializable]
    public class FleetCancelationData
    {
        public int Index { get; set; }
    }
}