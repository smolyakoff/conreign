using System;
using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using Conreign.Core.Contracts.Client.Exceptions;
using MediatR;
using Serilog;
using Serilog.Events;
using SerilogMetrics;

namespace Conreign.Client.Handler.Handlers.Decorators
{
    internal class ErrorLoggingDecorator<TCommand, TResponse> : ICommandHandler<TCommand, TResponse> where TCommand : IAsyncRequest<TResponse>
    {
        private readonly IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> _next;
        private readonly ILogger _logger;
        private readonly ICounterMeasure _counter;

        public ErrorLoggingDecorator(IAsyncRequestHandler<CommandEnvelope<TCommand, TResponse>, TResponse> next)
        {
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
            _logger = Log.Logger.ForContext(GetType());
            _counter = _logger.CountOperation(DiagnosticConstants.ErrorCounterName);
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message)
        {
            var context = message.Context;
            try
            {
                return await _next.Handle(message);
            }
            catch (Exception ex)
            {
                _counter.Increment();
                var level = ex is UserException ? LogEventLevel.Warning : LogEventLevel.Error;
                _logger.Write(level, ex, "[{TraceId}-{UserId}] {Message}", 
                    context.Metadata.TraceId, 
                    context.UserId == null ? "Anonymous" : context.UserId.ToString(),
                    ex.Message);
                throw;
            }
        }
    }
}
