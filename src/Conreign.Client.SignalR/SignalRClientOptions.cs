using System;
using Microsoft.AspNet.SignalR.Client;

namespace Conreign.Client.SignalR
{
    public class SignalRClientOptions
    {
        public SignalRClientOptions(string connectionUri, bool isDebug = false)
        {
            if (string.IsNullOrEmpty(connectionUri))
            {
                throw new ArgumentException("Connection uri cannot be null or empty.", nameof(connectionUri));
            }
            ConnectionUri = connectionUri;
            TraceLevels = isDebug ? TraceLevels.All : TraceLevels.StateChanges;
            IsDebug = isDebug;
        }

        public string ConnectionUri { get; }
        public bool IsDebug { get; set; }
        public TraceLevels TraceLevels { get; set; }
    }
}