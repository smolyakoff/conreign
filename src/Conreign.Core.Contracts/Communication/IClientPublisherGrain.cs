using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Contracts.Communication
{
    public interface IClientPublisherGrain : IGrainWithGuidCompoundKey, IClientPublisher, IConnectable
    {

    }
}