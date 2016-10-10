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
            var adapter = new EventHandlerAdapter<T>(handler);
            return _grain.Subscribe(typeof(T), adapter);
        }

        public Task Unsubscribe<T>(IEventHandler<T> handler) where T : class, ISystemEvent
        {
            var adapter = new EventHandlerAdapter<T>(handler);
            return _grain.Unsubscribe(typeof(T), adapter);
        }

        public Task Notify(params ISystemEvent[] events)
        {
            return _grain.Notify(events);
        }
    }
}
