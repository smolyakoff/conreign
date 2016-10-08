using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Contracts.Communication
{
    public interface IObserverGrain : IGrainWithGuidCompoundKey, IObserver, IConnectable
    {

    }
}