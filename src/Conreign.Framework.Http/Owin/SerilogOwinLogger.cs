using System;
using System.Diagnostics;
using Serilog;
using Serilog.Events;
using ILogger = Microsoft.Owin.Logging.ILogger;

namespace Conreign.Framework.Http.Owin
{
    public class SerilogOwinLogger : ILogger
    {
        private readonly Serilog.ILogger _logger;

        public SerilogOwinLogger(string name)
        {
            _logger = Log.ForContext("OwinLoggerName", name);
        }

        public bool WriteCore(TraceEventType eventType, int eventId, object state, Exception exception,
            Func<object, Exception, string> formatter)
        {
            var level = MapLevel(eventType);
            if (!_logger.IsEnabled(level))
            {
                return false;
            }
            if (exception != null)
            {
                var message = formatter(state, exception);
                _logger.Write(level, exception, "{Message}. EventId: {EventId}", message, eventId);
            }
            else if (state != null)
            {
                var message = formatter(state, null);
                _logger.Write(level, "{Message}. EventId: {EventId}", message, eventId);
            }
            else
            {
                _logger.Write(LogEventLevel.Verbose, "Owin logger state is null. EventId: {EventId}", eventId);
                return false;
            }
            return true;
        }

        private static LogEventLevel MapLevel(TraceEventType eventType)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (eventType)
            {
                case TraceEventType.Critical:
                    return LogEventLevel.Fatal;
                case TraceEventType.Error:
                    return LogEventLevel.Error;
                case TraceEventType.Warning:
                    return LogEventLevel.Warning;
                case TraceEventType.Information:
                    return LogEventLevel.Information;
                case TraceEventType.Verbose:
                    return LogEventLevel.Verbose;
                default:
                    return LogEventLevel.Debug;
            }
        }
    }
}