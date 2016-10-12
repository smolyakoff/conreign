using Conreign.Core.Contracts.Communication.Events;
using Orleans;

namespace Conreign.Core.Contracts.Communication
{
    public interface IClientPublisherGrain : IGrainWithGuidCompoundKey, IClientPublisher, IEventHandler<Connected>, IEventHandler<Disconnected>
    {
    }
}