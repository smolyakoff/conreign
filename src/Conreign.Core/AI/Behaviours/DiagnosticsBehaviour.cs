using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Serilog;
using Serilog.Context;
using SerilogMetrics;

namespace Conreign.Core.AI.Behaviours
{
    internal sealed class DiagnosticsBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly IBotBehaviour<IClientEvent> _next;
        private ICounterMeasure _counter;

        public DiagnosticsBehaviour(IBotBehaviour<IClientEvent> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task Handle(IBotNotification<IClientEvent> notification)
        {
            var context = notification.Context;
            var @event = notification.Event;
            _counter = _counter ?? context.Logger.CountOperation("BotHandledEvents");
            using (LogContext.PushProperty("EventType", @event.GetType()))
            {
                var id = $"{context.ReadableId}:{context.UserId}:{Guid.NewGuid()}";
                context.Logger.Debug("[{ReadableId}-{UserId}]: Started to handle {EventType}.",
                    context.ReadableId,
                    context.UserId,
                    @event.GetType());
                try
                {
                    using (context.Logger.BeginTimedOperation($"Bot handles client event {@event.GetType().Name}", id))
                    {
                        await _next.Handle(notification);
                    }
                }
                finally 
                {
                    _counter.Increment();
                    context.Logger.Debug("[{ReadableId}-{UserId}]: Finished to handle {EventType}.",
                        context.ReadableId,
                        context.UserId,
                        @event.GetType());
                }
            }
        }
    }
}