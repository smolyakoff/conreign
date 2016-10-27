﻿using System;
using System.IO;
using System.Threading.Tasks;
using Conreign.Core.Client.Exceptions;
using Conreign.Core.Contracts.Client;
using Orleans;
using Polly;

namespace Conreign.Core.Client
{
    public sealed class OrleansGameClient : IGameClient, IDisposable
    {
        private readonly IGrainFactory _factory;
        private bool _isDisposed;

        private OrleansGameClient(IGrainFactory factory)
        {
            _factory = factory;
        }

        public static Task<OrleansGameClient> Initialize(string configFilePath)
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
                    .WaitAndRetry(5, attempt => TimeSpan.FromSeconds(attempt * 3));
                var result = policy.ExecuteAndCapture(() => GrainClient.Initialize(configFilePath));
                if (result.Outcome == OutcomeType.Failure)
                {
                    throw new ConnectionException($"Failed to connect to the cluster: {result.FinalException.Message}", result.FinalException);
                }
            }
            var client = new OrleansGameClient(GrainClient.GrainFactory);
            return Task.FromResult(client);
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

        private void EnsureIsNotDisposed()
        {
            if (_isDisposed)
            {
                throw new ObjectDisposedException("GameClient");
            }
        }
    }
}
