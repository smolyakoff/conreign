using System;
using System.Threading.Tasks;
using Orleans;

namespace Conreign.Core.Contracts.Communication
{
    public interface IBusGrain : IGrainWithStringKey, ISystemPublisher
    {
        Task Subscribe(Type baseType, IEventHandler<ISystemEvent> handler);
        Task Unsubscribe(Type baseType, IEventHandler<ISystemEvent> handler);
    }
}
