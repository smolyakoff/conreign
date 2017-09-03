using System;
using Conreign.Contracts.Communication;

namespace Conreign.Server.Communication
{
    internal static class EventExtensions
    {
        public static EventEnvelope ToEnvelope(this IClientEvent @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            return new EventEnvelope(@event, @event.GetType().Name);
        }
    }
}