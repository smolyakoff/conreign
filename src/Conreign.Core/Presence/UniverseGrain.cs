using System.Threading.Tasks;
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
            return Task.CompletedTask;
        }

        public Task Connect(ConnectCommand command)
        {
            return _universe.Connect(command);
        }

        public Task Disconnect(DisconnectCommand command)
        {
            return _universe.Disconnect(command);
        }
    }
}