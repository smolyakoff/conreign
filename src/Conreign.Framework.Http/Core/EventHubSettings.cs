using System;
using Conreign.Framework.Contracts.Routing;

namespace Conreign.Framework.Http.Core
{
    public class EventHubSettings
    {
        public EventHubSettings()
        {
            StreamProviderName = "Default";
            StreamKey = new StreamKey(Guid.Empty, null);
        }

        public string StreamProviderName { get; set; }

        public StreamKey StreamKey { get; set; }

        internal void EnsureIsValid()
        {
            if (string.IsNullOrEmpty(StreamProviderName))
            {
                throw new InvalidOperationException("Stream provider name should not be empty.");
            }
            if (StreamKey == null)
            {
                throw new InvalidOperationException("Stream key should not be empty.");
            }
        }
    }
}