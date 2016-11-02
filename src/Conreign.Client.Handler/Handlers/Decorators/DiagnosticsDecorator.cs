using System;
using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using MediatR;
using Serilog;
using Serilog.Context;
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

        public DiagnosticsDecorator(IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _logger = Log.Logger.ForContext(GetType());
            _received = Log.Logger.CountOperation(
                DiagnosticConstants.ReceivedCommandsCounterName, 
                resolution: DiagnosticConstants.CommandsCounterResolution);
            _processed = Log.Logger.CountOperation(
                DiagnosticConstants.ProcessedCommandsCounterName,
                resolution: DiagnosticConstants.CommandsCounterResolution);
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message)
        {
            var context = message.Context;
            using (LogContext.PushProperties(
                new PropertyEnricher("TraceId", context.Metadata.TraceId),
                new PropertyEnricher("UserId", context.UserId),
                new PropertyEnricher("ConnectionId", context.Connection.Id),
                new PropertyEnricher("CommandType", message.Command.GetType())))
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
                    var id = DiagnosticConstants.HandleOperationId(context.Metadata.TraceId, message.Command.GetType());
                    using (_logger.BeginTimedOperation(DiagnosticConstants.HandleOperationDescription, id))
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
