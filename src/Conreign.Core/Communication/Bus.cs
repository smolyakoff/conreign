using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Communication
{
    public class Bus : IBus
    {
        private readonly IBusGrain _grain;

        public Bus(IBusGrain grain)
        {
            _grain = grain;
        }

        public Task Subscribe<T>(IEventHandler<T> handler) where T : class, ISystemEvent
        {
            return _grain.Subscribe(typeof(T), handler);
        }

        public Task Unsubscribe<T>(IEventHandler<T> handler) where T : class, ISystemEvent
        {
            return _grain.Unsubscribe(typeof(T), handler);
        }

        public Task UnsubscribeAll(IEventHandler handler)
        {
            return _grain.UnsubscribeAll(handler);
        }

        public Task Notify(params ISystemEvent[] events)
        {
            return _grain.Notify(events);
        }
    }
}
