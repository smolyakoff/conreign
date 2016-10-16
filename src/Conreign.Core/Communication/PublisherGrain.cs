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
        private IDictionary<Guid, IAsyncStream<IClientEvent>> _clientStreams;
        private IAsyncStream<IServerEvent> _serverStream;

        public override async Task OnActivateAsync()
        {
            _clientStreams = new Dictionary<Guid, IAsyncStream<IClientEvent>>();
            _streamProvider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            _serverStream = _streamProvider.GetStream<IServerEvent>(Guid.Empty, this.GetPrimaryKeyString());
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
            var clientTasks = _clientStreams
                .Keys
                .SelectMany(connectionId => clientEvents.Select(e => _clientStreams[connectionId].OnNextAsync(e)));
            var serverTasks = systemEvents
                .Select(e => _serverStream.OnNextAsync(e))
                .ToList();
            return Task.WhenAll(serverTasks.Concat(clientTasks));
        }

        public Task Handle(Connected @event)
        {
            var connectionId = @event.ConnectionId;
            if (_clientStreams.ContainsKey(connectionId))
            {
                return Task.CompletedTask;
            }
            _clientStreams[connectionId] = CreateStream(connectionId);
            return Task.CompletedTask;
        }

        public async Task Handle(Disconnected @event)
        {
            var connectionId = @event.ConnectionId;
            IAsyncStream<IClientEvent> stream;
            var isConnected = _clientStreams.TryGetValue(connectionId, out stream);
            if (isConnected)
            {
                _clientStreams.Remove(connectionId);
                await stream.OnCompletedAsync();
            }
        }

        private IAsyncStream<IClientEvent> CreateStream(Guid connectionId)
        {
            return _streamProvider.GetStream<IClientEvent>(connectionId, StreamConstants.ClientStreamNamespace);
        }
    }
}