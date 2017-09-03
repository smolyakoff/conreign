using System;
using Conreign.Contracts.Communication;

namespace Conreign.LoadTest.Core
{
    public class BotNotification<TEvent> : IBotNotification<TEvent> where TEvent : IClientEvent
    {
        public BotNotification(TEvent @event, BotContext context)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            Event = @event;
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public TEvent Event { get; }
        public BotContext Context { get; }
    }
}