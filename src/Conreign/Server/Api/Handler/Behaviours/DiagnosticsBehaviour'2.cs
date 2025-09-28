using MediatR;
using Serilog;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using SerilogMetrics;

namespace Conreign.Server.Api.Handler.Behaviours;

public class DiagnosticsBehaviour<TCommand, TResponse> : IPipelineBehavior<TCommand, TResponse>
    where TCommand : IRequest<TResponse>
{
    private const string ReceivedCounterName = "Handler.Received";
    private const string ProcessedCounterName = "Handler.Processed";
    private const string OperationDescription = "Handler.Handle";
    private const int CounterResolution = 10;
    private readonly HandlerContext _context;
    private readonly ILogger _logger;
    private readonly ICounterMeasure _processed;
    private readonly ICounterMeasure _received;

    public DiagnosticsBehaviour(HandlerContext context)
    {
        _context = context;
        _logger = Log.Logger.ForContext(GetType());
        _received = Log.Logger.CountOperation(
            ReceivedCounterName,
            resolution: CounterResolution);
        _processed = Log.Logger.CountOperation(
            ProcessedCounterName,
            resolution: CounterResolution);
    }

    public async Task<TResponse> Handle(TCommand command,
        CancellationToken cancellationToken,
        RequestHandlerDelegate<TResponse> next)
    {
        var diagnosticProperties = new ILogEventEnricher[]
        {
            new PropertyEnricher("TraceId", _context.Metadata.TraceId),
            new PropertyEnricher("ConnectionId", _context.Connection.Id),
            new PropertyEnricher("CommandType", command.GetType().Name)
        };
        using (LogContext.Push(diagnosticProperties))
        {
            try
            {
                _received.Increment();
                _logger.Debug("[Handler:{TraceId}] Received {CommandType}: {@Command}",
                    _context.Metadata.TraceId,
                    typeof(TCommand).Name,
                    command);
                TResponse response;
                using (_logger.BeginTimedOperation(OperationDescription))
                {
                    response = await next.Invoke();
                }

                _logger.Debug("[Handler:{TraceId}] Handled {CommandType} successfully: {@Response}",
                    _context.Metadata.TraceId,
                    typeof(TCommand).Name,
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