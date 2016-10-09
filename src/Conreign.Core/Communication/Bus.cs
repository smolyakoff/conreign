using System;
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

        [Serializable]
        private class EventHandlerAdapter<T> : IEventHandler<ISystemEvent>, IEquatable<EventHandlerAdapter<T>>
            where T : class, ISystemEvent
        {
            public EventHandlerAdapter(IEventHandler<T> handler)
            {
                _handler = handler;
            }

            public Task Handle(ISystemEvent @event)
            {
                return _handler.Handle((T) @event);
            }

            public bool Equals(EventHandlerAdapter<T> other)
            {
                if (ReferenceEquals(null, other))
                {
                    return false;
                }
                if (ReferenceEquals(this, other))
                {
                    return true;
                }
                return _handler.Equals(other._handler);
            }

            public override bool Equals(object obj)
            {
                if (ReferenceEquals(null, obj))
                {
                    return false;
                }
                if (ReferenceEquals(this, obj))
                {
                    return true;
                }
                if (obj.GetType() != GetType())
                {
                    return false;
                }
                return Equals((EventHandlerAdapter<T>) obj);
            }

            public override int GetHashCode()
            {
                return _handler.GetHashCode();
            }

            private readonly IEventHandler<T> _handler;
        }
    }
}
