using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Orleans;
using Orleans.Placement;
using Orleans.Streams;

namespace Conreign.Core.Communication
{
    [PreferLocalPlacement]
    public class ClientPublisherGrain : Grain, IClientPublisherGrain
    {
        private IStreamProvider _streamProvider;
        private IDictionary<Guid, IAsyncStream<IClientEvent>> _streams;

        public override async Task OnActivateAsync()
        {
            _streams = new Dictionary<Guid, IAsyncStream<IClientEvent>>();
            _streamProvider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            await base.OnActivateAsync();
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

        public Task Handle(Connected @event)
        {
            var connectionId = @event.ConnectionId;
            if (_streams.ContainsKey(connectionId))
            {
                return Task.CompletedTask;
            }
            _streams[connectionId] = CreateStream(connectionId);
            return Task.CompletedTask;
        }

        public async Task Handle(Disconnected @event)
        {
            var connectionId = @event.ConnectionId;
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