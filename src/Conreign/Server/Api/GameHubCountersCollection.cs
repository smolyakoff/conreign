using Serilog;
using SerilogMetrics;

namespace Conreign.Server.Api;

public class GameHubCountersCollection
{
    public GameHubCountersCollection(ILogger logger)
    {
        if (logger == null)
        {
            throw new ArgumentNullException(nameof(logger));
        }

        const int resolution = 10;
        EventsReceived = logger.CountOperation("Hub.EventsReceived", "event(s)", resolution: resolution);
        EventsDispatched = logger.CountOperation("Hub.EventsDispatched", "event(s)", resolution: resolution);
        ErrorsReceived = logger.CountOperation("Hub.ErrorsReceived", "error(s)", resolution: 1);
        ErrorsDispatched = logger.CountOperation("Hub.ErrorsDispatched", "error(s)", resolution: 1);
        StreamsCompleted = logger.CountOperation("Hub.StreamsCompleted", "stream(s)", resolution: 1);
    }

    public ICounterMeasure EventsReceived { get; }
    public ICounterMeasure EventsDispatched { get; }
    public ICounterMeasure ErrorsReceived { get; }
    public ICounterMeasure ErrorsDispatched { get; }
    public ICounterMeasure StreamsCompleted { get; }
}