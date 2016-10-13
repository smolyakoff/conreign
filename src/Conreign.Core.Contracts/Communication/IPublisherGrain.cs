using Conreign.Core.Contracts.Communication.Events;
using Orleans;

namespace Conreign.Core.Contracts.Communication
{
    public interface IPublisherGrain : IGrainWithStringKey, IPublisher<IEvent>, IEventHandler<Connected>, IEventHandler<Disconnected>
    {
    }
}