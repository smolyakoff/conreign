using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Shared.Presence;
using Orleans;

namespace Conreign.Server.Contracts.Server.Presence;

public interface IConnectionGrain : IGrainWithStringKey, IConnection, ITopicFactory
{
}