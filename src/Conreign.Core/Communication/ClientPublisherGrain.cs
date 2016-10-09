using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Communication
{
    public class ClientPublisherGrain : Grain, IClientPublisherGrain
    {
        private IStreamProvider _streamProvider;
        private IDictionary<Guid, IAsyncStream<IClientEvent>> _streams;

        public override Task OnActivateAsync()
        {
            _streamProvider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            _streams = new Dictionary<Guid, IAsyncStream<IClientEvent>>();
            return base.OnActivateAsync();
        }

        public Task Notify(params IClientEvent[] events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            var tasks = _streams.Keys
                .SelectMany(connectionId => events.Select(e => _streams[connectionId].OnNextAsync(e)))
                .ToArray();
            return Task.WhenAll(tasks);
        }

        public Task Connect(Guid connectionId)
        {
            if (_streams.ContainsKey(connectionId))
            {
                return Task.CompletedTask;
            }
            _streams[connectionId] = CreateStream(connectionId);
            return Task.CompletedTask;
        }

        public async Task Disconnect(Guid connectionId)
        {
            IAsyncStream<IClientEvent> stream;
            var isConnected = _streams.TryGetValue(connectionId, out stream);
            if (isConnected)
            {
                _streams.Remove(connectionId);
                await stream.OnCompletedAsync();
            }
        }

        private IAsyncStream<IClientEvent> CreateStream(Guid connectionId)
        {
            return _streamProvider.GetStream<IClientEvent>(connectionId, StreamConstants.ClientStreamNamespace);
        }
    }
}