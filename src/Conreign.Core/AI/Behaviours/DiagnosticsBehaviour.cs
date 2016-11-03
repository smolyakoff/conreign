using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using SerilogMetrics;

namespace Conreign.Core.AI.Behaviours
{
    internal sealed class DiagnosticsBehaviour : IBotBehaviour<IClientEvent>
    {
        private readonly IBotBehaviour<IClientEvent> _next;
        private ICounterMeasure _processed;
        private const string OperationDescription = "Bot.Handle";
        private const string ProcessedCounterName = "Bot.ProcessedEvents";
        private const int ProcessedCounterResolution = 10;

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
            _processed = _processed ?? context.Logger.CountOperation(ProcessedCounterName, resolution: ProcessedCounterResolution);
            var diagnosticProperties = new ILogEventEnricher[]
            {
                new PropertyEnricher("ConnectionId", context.Connection.Id),
                new PropertyEnricher("BotId", context.BotId),
                new PropertyEnricher("UserId", context.UserId),
                new PropertyEnricher("EventType", notification.Event.GetType().Name)
            };
            using (LogContext.PushProperties(diagnosticProperties))
            {
                context.Logger.Debug("[Bot:{BotId}:{UserId}] Started to handle {EventType}.",
                    context.BotId,
                    context.UserId,
                    @event.GetType().Name);
                try
                {
                    using (context.Logger.BeginTimedOperation(OperationDescription))
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