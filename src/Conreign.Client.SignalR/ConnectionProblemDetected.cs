using System;
using Conreign.Contracts.Communication;

namespace Conreign.Client.SignalR
{
    public class ConnectionProblemDetected : IClientEvent
    {
        public ConnectionProblemDetected(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            Exception = exception;
        }

        public Exception Exception { get; }
        public DateTime Timestamp { get; }
    }
}