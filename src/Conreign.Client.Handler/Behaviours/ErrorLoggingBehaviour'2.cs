using System;
using System.Threading.Tasks;
using Conreign.Contracts.Errors;
using MediatR;
using Serilog;
using Serilog.Events;
using SerilogMetrics;

namespace Conreign.Client.Handler.Behaviours
{
    public class ErrorLoggingBehaviour<TCommand, TResponse> : ICommandPipelineBehaviour<TCommand, TResponse>
        where TCommand : IRequest<TResponse>
    {
        private const string ErrorsCounterName = "Handler.Errors";
        private readonly ICounterMeasure _counter;
        private readonly ILogger _logger;

        public ErrorLoggingBehaviour()
        {
            _logger = Log.Logger.ForContext(GetType());
            _counter = _logger.CountOperation(ErrorsCounterName);
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message,
            RequestHandlerDelegate<TResponse> next)
        {
            var context = message.Context;
            try
            {
                return await next.Invoke();
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