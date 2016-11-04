using System;
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
        private readonly IOrleansClientInitializer _initializer;
        private bool _isDisposed;

        private OrleansClient(IGrainFactory factory, IOrleansClientInitializer initializer)
        {
            _factory = factory;
            _initializer = initializer;
        }

        public async Task<IClientConnection> Connect(Guid connectionId)
        {
            EnsureIsNotDisposed();
            return await OrleansClientConnection.Initialize(_factory, connectionId);
        }

        public void Dispose()
        {
            _isDisposed = true;
            _initializer.Uninitialize();
        }

        public static Task<OrleansClient> Initialize(IOrleansClientInitializer initializer)
        {
            if (initializer == null)
            {
                throw new ArgumentNullException(nameof(initializer));
            }
            var policy = Policy
                    .Handle<Exception>()
                    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(attempt * 3));
            var result = policy.ExecuteAndCapture(initializer.Initialize);
            if (result.Outcome == OutcomeType.Failure)
            {
                throw new ConnectionException($"Failed to connect to the cluster: {result.FinalException.Message}",
                    result.FinalException);
            }
            var client = new OrleansClient(GrainClient.GrainFactory, initializer);
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