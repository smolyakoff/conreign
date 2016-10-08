using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Communication
{
    public class ObserverGrain : Grain, IObserverGrain
    {
        private IStreamProvider _streamProvider;
        private IDictionary<Guid, IAsyncStream<object>> _streams;

        public override Task OnActivateAsync()
        {
            _streamProvider = GetStreamProvider(StreamConstants.ClientStreamProviderName);
            _streams = new Dictionary<Guid, IAsyncStream<object>>();
            return base.OnActivateAsync();
        }

        public Task Notify(object @event)
        {
            var tasks = _streams.Keys
                .Select(x => _streams[x].OnNextAsync(@event))
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

        public Task Disconnect(Guid connectionId)
        {
            _streams.Remove(connectionId);
            return Task.CompletedTask;
        }

        private IAsyncStream<object> CreateStream(Guid connectionId)
        {
            return _streamProvider.GetStream<object>(connectionId, StreamConstants.ClientStreamNamespace);
        }
    }
}