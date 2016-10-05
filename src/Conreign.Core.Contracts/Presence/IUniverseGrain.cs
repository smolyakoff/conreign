using Orleans;

namespace Conreign.Core.Contracts.Presence
{
    public interface IUniverseGrain : IGrainWithIntegerKey, IUniverse, IConnectable
    {
    }
}