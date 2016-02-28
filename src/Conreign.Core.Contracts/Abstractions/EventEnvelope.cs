using System.Collections.Generic;
using System.Collections.Immutable;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Abstractions
{
    [Immutable]
    public class EventEnvelope<T>
    {
        public EventEnvelope(T @event, IEnumerable<string> connectionIds = null)
        {
            Event = @event;
            ConnectionIds = connectionIds?.ToImmutableHashSet();
        }

        public T Event { get; }

        public ImmutableHashSet<string> ConnectionIds { get; }
    }
}