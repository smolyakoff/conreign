using System;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.AI
{
    public class BotNotification<TEvent> : IBotNotification<TEvent> where TEvent : IClientEvent
    {
        public BotNotification(TEvent @event, BotContext context)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            Event = @event;
            Context = context;
        }

        public TEvent Event { get; }
        public BotContext Context { get; }
    }
}