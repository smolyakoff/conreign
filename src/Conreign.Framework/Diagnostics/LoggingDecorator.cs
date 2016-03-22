using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using MediatR;
using Serilog;
using Serilog.Events;

namespace Conreign.Framework.Diagnostics
{
    public class LoggingDecorator : IAsyncRequestHandler<Request, Response>
    {
        private readonly ILogger _logger;
        private readonly IAsyncRequestHandler<Request, Response> _next;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public LoggingDecorator(IAsyncRequestHandler<Request, Response> next)
        {
            _logger = Log.Logger.ForContext(typeof (LoggingDecorator));
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task<Response> Handle(Request message)
        {
            var traceId = Guid.NewGuid();
            message.Meta = message.Meta ?? new Dictionary<string, object>();
            message.Meta["trace"] = new {TraceId = traceId};
            var logger = _logger.ForContext("TraceId", traceId).ForContext("ActionType", message.Type);
            if (logger.IsEnabled(LogEventLevel.Debug))
            {
                logger.Debug("Action [{ActionType}] received: {@Action}", message.Type, message);
            }
            try
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                var response = await _next.Handle(message);
                LogEventLevel level;
                string text;
                object data;
                switch (response.Status)
                {
                    case ResponseStatus.UserError:
                        level = LogEventLevel.Debug;
                        text = "Action handler [{ActionType}] returned user error: {@Data}";
                        data = response.Error;
                        break;
                    case ResponseStatus.Failure:
                        level = LogEventLevel.Warning;
                        text = "Action handler [{ActionType}] returned failure: {@Data}";
                        data = response.Error;
                        break;
                    default:
                        text = "Action handler [{ActionType}] returned success: {@Data}";
                        level = LogEventLevel.Verbose;
                        data = response.Result;
                        break;
                }
                if (logger.IsEnabled(level))
                {
                    logger.Write(level, text, message.Type, data);
                }
                return response;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Action handler [{ActionType}] throwed an exception: {Message}", message.Type,
                    ex.Message);
                throw;
            }
            finally
            {
                _logger.Debug("Action handler [{ActionType}] finished in {Time}.", message.Type, _stopwatch.Elapsed);
                _stopwatch.Stop();
            }
        }
    }
}