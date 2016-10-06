using System;
using System.IO;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;
using Orleans.Streams;
using Polly;

namespace Conreign.Core.Client
{
    public class GameClient
    {
        private readonly IGrainFactory _factory;
        private readonly Lazy<IUniverse> _universe;
        private StreamSubscriptionHandle<MessageEnvelope> _subscription;

        private GameClient(IGrainFactory factory)
        {
            _factory = factory;
        }

        public static async Task<GameClient> Initialize(string configFilePath)
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
                policy.Execute(() => GrainClient.Initialize(configFilePath));
            }
            var client = new GameClient(GrainClient.GrainFactory);
            var stream = GrainClient
                .GetStreamProvider(StreamConstants.DefaultProviderName)
                .GetStream<MessageEnvelope>(StreamConstants.ClientStreamKey, StreamConstants.ClientStreamNamespace);
            client._subscription = await stream.SubscribeAsync(client.OnNext, client.OnError, client.OnCompleted);
            return client;
        }

        public Task Disconnect(string connectionId)
        {
            throw new NotImplementedException();
        }

        public Task<IPlayer> Connect(string accessToken = null)
        {
            throw new NotImplementedException();
        }

        private Task OnCompleted()
        {
            throw new NotImplementedException();
        }

        private Task OnNext(MessageEnvelope envelope, StreamSequenceToken token)
        {
            throw new NotImplementedException();
        }

        private Task OnError(Exception ex)
        {
            throw new NotImplementedException();
        }
    }
}
