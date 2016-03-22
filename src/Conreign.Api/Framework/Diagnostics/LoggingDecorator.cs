using System;
using System.Diagnostics;
using System.Threading.Tasks;
using MediatR;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Events;

namespace Conreign.Api.Framework.Diagnostics
{
    public class LoggingDecorator : IAsyncRequestHandler<HttpAction, HttpActionResult>
    {
        private readonly IAsyncRequestHandler<HttpAction, HttpActionResult> _next;

        private readonly ILogger _logger;
        private readonly Stopwatch _stopwatch = new Stopwatch();

        public LoggingDecorator(IAsyncRequestHandler<HttpAction, HttpActionResult> next)
        {
            _logger = Log.Logger.ForContext(typeof(LoggingDecorator));
            if (next == null)
            {
                throw new ArgumentNullException(nameof(next));
            }
            _next = next;
        }

        public async Task<HttpActionResult> Handle(HttpAction message)
        {
            var traceId = Guid.NewGuid();
            message.Meta = message.Meta ?? new JObject();
            message.Meta["TraceId"] = new JValue(traceId);
            var logger = _logger.ForContext("TraceId", traceId).ForContext("ActionType", message.Type);
            if (logger.IsEnabled(LogEventLevel.Debug))
            {
                logger.Debug("Action [{ActionType}] received: {@Action}", message.Type, message);
            }
            try
            {
                _stopwatch.Reset();
                _stopwatch.Start();
                var result = await _next.Handle(message);
                var status = ((int) result.StatusCode)/100;
                LogEventLevel level;
                string text;
                switch (status)
                {
                    case 4:
                        level = LogEventLevel.Debug;
                        text = "Action handler [{ActionType}] returned user error: {@Data}";
                        break;
                    case 5:
                        level = LogEventLevel.Warning;
                        text = "Action handler [{ActionType}] returned server error: {@Data}";
                        break;
                    default:
                        text = "Action handler [{ActionType}] returned success: {@Data}";
                        level = LogEventLevel.Verbose;
                        break;
                }
                if (logger.IsEnabled(level))
                {
                    logger.Write(level, text, message.Type, result);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Action handler [{ActionType}] throwed an exception: {Message}", message.Type, ex.Message);
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
