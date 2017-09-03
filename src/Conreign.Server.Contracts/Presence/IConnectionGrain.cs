using Conreign.Contracts.Presence;
using Conreign.Server.Contracts.Communication;
using Orleans;

namespace Conreign.Server.Contracts.Presence
{
    public interface IConnectionGrain : IGrainWithGuidKey, IConnection, ITopicFactory
    {
    }
}