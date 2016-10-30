using System;
using Orleans;
using Orleans.Runtime.Configuration;

namespace Conreign.Client.Orleans
{
    public class OrleansClientInitializer : IOrleansClientInitializer
    {
        private readonly ClientConfiguration _configuration;

        public OrleansClientInitializer(ClientConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            _configuration = configuration;
        }

        public void Initialize()
        {
            if (GrainClient.IsInitialized)
            {
                return;
            }
            GrainClient.Initialize(_configuration);
        }

        public void Uninitialize()
        {
            if (!GrainClient.IsInitialized)
            {
                return;
            }
            GrainClient.Uninitialize();
        }
    }
}