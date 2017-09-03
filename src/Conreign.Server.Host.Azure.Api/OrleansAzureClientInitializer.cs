using System;
using Conreign.Client.Orleans;
using Orleans.Runtime.Configuration;
using Orleans.Runtime.Host;

namespace Conreign.Server.Host.Azure.Api
{
    public class OrleansAzureClientInitializer : IOrleansClientInitializer
    {
        private readonly ClientConfiguration _config;

        public OrleansAzureClientInitializer(ClientConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            _config = config;
        }

        public void Initialize()
        {
            if (AzureClient.IsInitialized)
            {
                return;
            }
            AzureClient.Initialize(_config);
        }

        public void Uninitialize()
        {
            if (!AzureClient.IsInitialized)
            {
                return;
            }
            AzureClient.Uninitialize();
        }
    }
}