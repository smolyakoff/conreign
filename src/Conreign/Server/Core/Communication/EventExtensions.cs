using Conreign.Server.Contracts.Shared.Communication;

namespace Conreign.Server.Core.Communication;

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