using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

[Serializable]
[Immutable]
public class FleetCancelationData
{
    public int Index { get; set; }
}