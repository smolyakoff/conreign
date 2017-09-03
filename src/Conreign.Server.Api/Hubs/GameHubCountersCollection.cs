using System;
using Serilog;
using SerilogMetrics;

namespace Conreign.Server.Api.Hubs
{
    public class GameHubCountersCollection
    {
        public GameHubCountersCollection(ILogger logger)
        {
            if (logger == null)
            {
                throw new ArgumentNullException(nameof(logger));
            }
            const int resolution = 10;
            EventsReceived = Log.Logger.CountOperation("Hub.EventsReceived", "event(s)", resolution: resolution);
            EventsDispatched = Log.Logger.CountOperation("Hub.EventsDispatched", "event(s)", resolution: resolution);
            ErrorsReceived = Log.Logger.CountOperation("Hub.ErrorsReceived", "error(s)", resolution: 1);
            ErrorsDispatched = Log.Logger.CountOperation("Hub.ErrorsDispatched", "error(s)", resolution: 1);
            StreamsCompleted = Log.Logger.CountOperation("Hub.StreamsCompleted", "stream(s)", resolution: 1);
        }

        public ICounterMeasure EventsReceived { get; }
        public ICounterMeasure EventsDispatched { get; }
        public ICounterMeasure ErrorsReceived { get; }
        public ICounterMeasure ErrorsDispatched { get; }
        public ICounterMeasure StreamsCompleted { get; }
    }
}