using System;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Gameplay;
using Orleans;
using Orleans.Streams;

namespace Conreign.Core.Presence
{
    public class UniverseGrain : Grain<UniverseState>, IUniverseGrain
    {
        private Universe _universe;
        private StreamSubscriptionHandle<IServerEvent> _subscription;

        public override async Task OnActivateAsync()
        {
            _universe = new Universe(State);
            var stream = GetStreamProvider(StreamConstants.ClientStreamProviderName)
                .GetStream<IServerEvent>(Guid.Empty, ServerTopics.Global);
            _subscription = await this.EnsureIsSubscribedOnce(stream);
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _subscription.UnsubscribeAsync();
            await base.OnDeactivateAsync();
        }

        public Task Disconnect(Guid connectionId)
        {
            return _universe.Disconnect(connectionId);
        }

        public Task Ping()
        {
            return Task.CompletedTask;
        }

        public Task Handle(Connected @event)
        {
            return _universe.Handle(@event);
        }
    }
}