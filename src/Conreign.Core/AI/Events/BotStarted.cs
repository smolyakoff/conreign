using System;

namespace Conreign.Core.AI.Events
{
    public class BotStarted : IBotEvent
    {
        public DateTime Timestamp { get; }
    }
}