using System;
using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using MediatR;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using SerilogMetrics;

namespace Conreign.Client.Handler.Handlers.Decorators
{
    internal class DiagnosticsDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> _next;
        private readonly ILogger _logger;
        private readonly ICounterMeasure _received;
        private readonly ICounterMeasure _processed;
        private const string ReceivedCounterName = "Handler.Received";
        private const string ProcessedCounterName = "Handler.Processed";
        private const string OperationDescription = "Handler.Handle";
        private const int CounterResolution = 10;

        public DiagnosticsDecorator(IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _logger = Log.Logger.ForContext(GetType());
            _received = Log.Logger.CountOperation(
                ReceivedCounterName, 
                resolution: CounterResolution);
            _processed = Log.Logger.CountOperation(
                ProcessedCounterName,
                resolution: CounterResolution);
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message)
        {
            var context = message.Context;
            var diagnosticProperties = new ILogEventEnricher[]
            {
                new PropertyEnricher("TraceId", context.Metadata.TraceId),
                new PropertyEnricher("UserId", context.UserId),
                new PropertyEnricher("ConnectionId", context.Connection.Id),
                new PropertyEnricher("CommandType", message.Command.GetType().Name)
            };
            using (LogContext.PushProperties(diagnosticProperties))
            {
                try
                {
                    _received.Increment();
                    _logger.Debug("[Handler:{TraceId}:{UserId}] Received {CommandType}: {@Command}", 
                        context.Metadata.TraceId,
                        context.UserId,
                        message.Command.GetType().Name,
                        message.Command);
                    TResponse response;
                    using (_logger.BeginTimedOperation(OperationDescription))
                    {
                        response = await _next.Handle(message);
                    }
                    _logger.Debug("[Handler:{TraceId}:{UserId}] Handled {CommandType} successfully: {@Response}",
                        context.Metadata.TraceId,
                        context.UserId,
                        message.Command.GetType().Name,
                        response);
                    return response;
                }
                finally
                {
                    _processed.Increment();
                }
            }
        }
    }
}
