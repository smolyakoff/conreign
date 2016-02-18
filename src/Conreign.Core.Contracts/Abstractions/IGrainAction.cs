using Orleans;

namespace Conreign.Core.Contracts.Abstractions
{
    public interface IGrainAction<TGrain> where TGrain : IGrain
    {
        GrainKey<TGrain> GrainKey { get; }
    }
}