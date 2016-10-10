using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using MongoDB.Bson.Serialization.Attributes;

namespace Conreign.Core.Communication
{
    [Serializable]
    [BsonDiscriminator("EventHandler")]
    public class EventHandlerAdapter<T> : IEventHandler<ISystemEvent>, IEquatable<EventHandlerAdapter<T>>
        where T : class, ISystemEvent
    {
        public EventHandlerAdapter(IEventHandler<T> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }
            InnerHandler = handler;
        }

        // Public only for serialization purposes
        public IEventHandler<T> InnerHandler { get; set; }

        public Task Handle(ISystemEvent @event)
        {
            return InnerHandler.Handle((T)@event);
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
            return InnerHandler.Equals(other.InnerHandler);
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
            return Equals((EventHandlerAdapter<T>)obj);
        }

        public override int GetHashCode()
        {
            return InnerHandler.GetHashCode();
        }
    }
}