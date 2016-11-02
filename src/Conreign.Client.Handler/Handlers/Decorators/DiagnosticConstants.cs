using System;

namespace Conreign.Client.Handler.Handlers.Decorators
{
    internal class DiagnosticConstants
    {
        public const string ErrorCounterName = "Handler.Counters.Errors";
        public static string HandleOperationDescription = "Handler.Operations.HandleCommand";
        public static string HandleOperationId(string traceId, Type commandType)
        {
            return $"{commandType.Name}:{traceId}";
        }
        public const string ReceivedCommandsCounterName = "Handler.Counters.ReceivedCommands";
        public const string ProcessedCommandsCounterName = "Handler.Counters.ProcessedCommands";
        public const int CommandsCounterResolution = 10;
    }
}
