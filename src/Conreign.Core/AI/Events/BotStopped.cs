using System;

namespace Conreign.Core.AI.Events
{
    public class BotStopped : IBotEvent
    {
        public DateTime Timestamp { get; }
    }
}