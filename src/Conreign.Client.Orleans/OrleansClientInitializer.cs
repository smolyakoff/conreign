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
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
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