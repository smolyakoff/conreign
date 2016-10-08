using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay;
using Conreign.Core.Contracts.Presence;
using Orleans;

namespace Conreign.Core.Presence
{
    public class UniverseGrain : Grain<UniverseState>, IUniverseGrain
    {
        private Universe _universe;

        public override Task OnActivateAsync()
        {
            _universe = new Universe(State);
            return base.OnActivateAsync();
        }

        public Task Disconnect(Guid connectionId)
        {
            return _universe.Disconnect(connectionId);
        }

        public Task Track(Guid connectionId, IConnectable connectable)
        {
            return _universe.Track(connectionId, connectable);
        }

        public Task Test(IPlayerGrain player)
        {
            return _universe.Track(Guid.NewGuid(), player);
        }
    }
}