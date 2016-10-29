using Conreign.Core.Contracts.Communication;
using Orleans;

namespace Conreign.Core.Contracts.Presence
{
    public interface IConnectionGrain : IGrainWithGuidKey, IConnection, ITopicFactory
    {
    }
}