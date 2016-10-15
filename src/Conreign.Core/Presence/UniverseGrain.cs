using System;
using System.Threading.Tasks;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Presence
{
    public class UniverseGrain : Grain<UniverseState>, IUniverseGrain
    {
        private Universe _universe;
        private IBus _bus;

        public override async Task OnActivateAsync()
        {
            _universe = new Universe(State);
            _bus = GrainFactory.GetGrain<IBusGrain>(ServerTopics.Global);
            await _bus.Subscribe(this.AsReference<IUniverseGrain>());
            RegisterTimer(SaveState, null, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(30));
            await base.OnActivateAsync();
        }

        public override async Task OnDeactivateAsync()
        {
            await _bus.Unsubscribe(this.AsReference<IUniverseGrain>());
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

        private Task SaveState(object arg)
        {
            return WriteStateAsync();
        }
    }
}