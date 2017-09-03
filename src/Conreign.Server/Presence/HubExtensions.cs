using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Server.Contracts.Communication;

namespace Conreign.Server.Presence
{
    public static class HubExtensions
    {
        public static Task NotifyEverybody(this IHub hub, IEnumerable<IEvent> events)
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            return hub.NotifyEverybody(events.ToArray());
        }

        public static Task Notify(this IHub hub, Guid userId, params IEvent[] events)
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            return hub.Notify(new HashSet<Guid> {userId}, events);
        }

        public static Task NotifyEverybodyExcept(this IHub hub, Guid userId, params IEvent[] events)
        {
            if (hub == null)
            {
                throw new ArgumentNullException(nameof(hub));
            }
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            return hub.NotifyEverybodyExcept(new HashSet<Guid> {userId}, events);
        }
    }
}