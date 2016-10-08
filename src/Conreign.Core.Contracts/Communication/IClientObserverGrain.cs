using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Contracts.Communication
{
    public interface IClientObserverGrain : IGrainWithGuidCompoundKey, IClientObserver, IConnectable
    {

    }
}