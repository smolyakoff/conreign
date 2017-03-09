using System;
using Conreign.Core.AI.Events;

namespace Conreign.Core.AI.Behaviours.Events
{
    public class BotAuthenticated : IBotEvent
    {
        public DateTime Timestamp { get; }
    }
}