using System;
using System.Threading.Tasks;
using Conreign.Client.Handler.Handlers.Common;
using Conreign.Core.Contracts.Client.Exceptions;
using MediatR;
using Serilog;
using Serilog.Events;
using SerilogMetrics;

namespace Conreign.Client.Handler.Handlers.Behaviours
{
    internal class ErrorLoggingBehaviour<TCommand, TResponse> : ICommandPipelineBehaviour<TCommand, TResponse> where TCommand : IRequest<TResponse>
    {
        private readonly ILogger _logger;
        private readonly ICounterMeasure _counter;
        private const string ErrorsCounterName = "Handler.Errors";

        public ErrorLoggingBehaviour()
        {
            _logger = Log.Logger.ForContext(GetType());
            _counter = _logger.CountOperation(ErrorsCounterName);
        }

        public async Task<TResponse> Handle(CommandEnvelope<TCommand, TResponse> message, RequestHandlerDelegate<TResponse> next)
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
