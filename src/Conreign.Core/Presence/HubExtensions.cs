using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Presence
{
    public static class HubExtensions
    {
        public static Task NotifyEverybody(this IHub hub, IEnumerable<IClientEvent> events)
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
    }
}
