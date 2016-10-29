using System;
using System.IO;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Client;
using Conreign.Core.Contracts.Client.Exceptions;
using Orleans;
using Polly;

namespace Conreign.Client.Orleans
{
    public sealed class OrleansClient : IClient, IDisposable
    {
        private readonly IGrainFactory _factory;
        private bool _isDisposed;

        private OrleansClient(IGrainFactory factory)
        {
            _factory = factory;
        }

        public async Task<IClientConnection> Connect(Guid connectionId)
        {
            EnsureIsNotDisposed();
            return await OrleansClientConnection.Initialize(_factory, connectionId);
        }

        public void Dispose()
        {
            if (_isDisposed || !GrainClient.IsInitialized)
            {
                return;
            }
            _isDisposed = true;
            GrainClient.Uninitialize();
        }

        public static Task<OrleansClient> Initialize(string configFilePath)
        {
            if (string.IsNullOrEmpty(configFilePath))
            {
                throw new ArgumentException("Config file path cannot be null or empty.", nameof(configFilePath));
            }
            if (!File.Exists(configFilePath))
            {
                throw new ArgumentException($"Orleans client config not found at: {Path.GetFullPath(configFilePath)}.");
            }
            if (!GrainClient.IsInitialized)
            {
                var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(attempt*3));
                var result = policy.ExecuteAndCapture(() => GrainClient.Initialize(configFilePath));
                if (result.Outcome == OutcomeType.Failure)
                {
                    throw new ConnectionException($"Failed to connect to the cluster: {result.FinalException.Message}",
                        result.FinalException);
                }
            }
            var client = new OrleansClient(GrainClient.GrainFactory);
            return Task.FromResult(client);
        }

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("GameClient");
            }
        }
    }
}