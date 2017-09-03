using System;

namespace Conreign.LoadTest.Core.Events
{
    public class BotStarted : IBotEvent
    {
        public DateTime Timestamp { get; }
    }
}