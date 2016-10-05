using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication
{
    [Immutable]
    public class NotifyEverybodyCommand
    {
        public NotifyEverybodyCommand(object @event)
        {
            if (@event == null)
            {
                throw new ArgumentNullException(nameof(@event));
            }
            Event = @event;
        }

        public object Event { get; set; }
    }
}