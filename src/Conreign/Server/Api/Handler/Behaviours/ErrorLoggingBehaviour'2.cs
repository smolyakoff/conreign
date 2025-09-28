using Conreign.Server.Contracts.Shared.Errors;
using MediatR;
using Serilog;
using Serilog.Events;
using SerilogMetrics;

namespace Conreign.Server.Api.Handler.Behaviours;

public class ErrorLoggingBehaviour<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    private const string ErrorsCounterName = "Handler.Errors";
    private readonly HandlerContext _context;
    private readonly ICounterMeasure _counter;
    private readonly ILogger _logger;

    public ErrorLoggingBehaviour(HandlerContext context)
    {
        _context = context;
        _logger = Log.Logger.ForContext(GetType());
        _counter = _logger.CountOperation(ErrorsCounterName);
    }

    public async Task<TResponse> Handle(TCommand message,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        try
        {
            return await next.Invoke();
        }
        catch (Exception ex)
        {
            _counter.Increment();
            var level = ex is UserException ? LogEventLevel.Warning : LogEventLevel.Error;
            _logger.Write(level, ex, "[{TraceId}-{UserId}] {Message}",
                _context.Metadata.TraceId,
                _context.UserId == null ? "Anonymous" : _context.UserId.ToString(),
                ex.Message);
            throw;
        }
    }
}