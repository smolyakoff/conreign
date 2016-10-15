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
    public class PublisherGrain : Grain, IPublisherGrain
    {
        private IStreamProvider _streamProvider;
        private IDictionary<Guid, IAsyncStream<IClientEvent>> _streams;
        private IBusGrain _bus;

        public override async Task OnActivateAsync()
        {
            _streams = new Dictionary<Guid, IAsyncStream<IClientEvent>>();
            _streamProvider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            _bus = GrainFactory.GetGrain<IBusGrain>(this.GetPrimaryKeyString());
            await base.OnActivateAsync();
        }

        public Task Notify(params IEvent[] events)
        {
            if (events == null)
            {
                throw new ArgumentNullException(nameof(events));
            }
            var clientEvents = events.OfType<IClientEvent>();
            var systemEvents = events.OfType<IServerEvent>();
            var clientTasks = _streams.Keys
                .SelectMany(connectionId => clientEvents.Select(e => _streams[connectionId].OnNextAsync(e)));
            var systemTasks = systemEvents.Select(e => _bus.Notify(e));
            var allTasks = systemTasks.Concat(clientTasks).ToList();
            return Task.WhenAll(allTasks);
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