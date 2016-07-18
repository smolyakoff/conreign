using Conreign.Framework.Contracts.Communication;

namespace Conreign.Core.Contracts.Communication
{
    public static class Event
    {
        public static Event<T> Create<T>(T payload, params string[] connectionIds)
        {
            return new Event<T>(payload, connectionIds);
        }
    }

    public class Event<T> : IEvent<T>
    {
        public Event(T payload, params string[] connectionIds)
        {
            Payload = payload;
            ConnectionIds = connectionIds.ToImmutableHashSet();
        }

        public IImmutableSet<string> ConnectionIds { get; set; }

        public T Payload { get; }
    }
}