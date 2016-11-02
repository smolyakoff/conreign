using System;

namespace Conreign.Client.SignalR
{
    internal static class DiagnosticConstants
    {
        public const string SendOperationDescription = "Conreign.Client.SignalR.Operations.Send";

        public static string SendOperationId(Type messageType)
        {
            return $"Conreign.Client.SignalR.Operations.Send[{messageType.Name}]";
        }
    }
}
