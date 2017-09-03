using System;
using Conreign.LoadTest.Core.Events;

namespace Conreign.LoadTest.Core.Behaviours.Events
{
    public class BotAuthenticated : IBotEvent
    {
        public DateTime Timestamp { get; }
    }
}