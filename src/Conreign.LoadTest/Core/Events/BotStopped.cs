using System;

namespace Conreign.LoadTest.Core.Events
{
    public class BotStopped : IBotEvent
    {
        public DateTime Timestamp { get; }
    }
}