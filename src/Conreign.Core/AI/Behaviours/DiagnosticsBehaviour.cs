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
        private ICounterMeasure _processed;

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
            _processed = _processed ?? context.Logger.CountOperation(DiagnosticsConstants.BotProcessedEventsCounterName(context.BotId));
            using (LogContext.PushProperty("EventType", @event.GetType()))
            {
                context.Logger.Debug("[Bot:{BotId}:{UserId}] Started to handle {EventType}.",
                    context.BotId,
                    context.UserId,
                    @event.GetType().Name);
                try
                {
                    var id = DiagnosticsConstants.BotHandleEventOperationId(@event.GetType());
                    using (context.Logger.BeginTimedOperation(DiagnosticsConstants.BotHandleEventOperationDescription, id))
                    {
                        await _next.Handle(notification);
                    }
                }
                finally 
                {
                    _processed.Increment();
                    context.Logger.Debug("[Bot:{BotId}:{UserId}] Finished to handle {EventType}.",
                        context.BotId,
                        context.UserId,
                        @event.GetType().Name);
                }
            }
        }
    }
}