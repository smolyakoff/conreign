using System.Threading.Tasks;
using Conreign.Server.Communication;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Presence;
using Orleans;

namespace Conreign.Server.Presence
{
    public class ConnectionGrain : Grain<ConnectionState>, IConnectionGrain
    {
        private Connection _connection;

        public Task Connect(string topicId)
        {
            return _connection.Connect(topicId);
        }

        public async Task Disconnect()
        {
            await _connection.Disconnect();
            DeactivateOnIdle();
        }

        public Task<ITopic> Create(string id)
        {
            var topic = new BroadcastTopic(GetStreamProvider(StreamConstants.ProviderName), id);
            return Task.FromResult((ITopic) topic);
        }

        public override async Task OnActivateAsync()
        {
            State.ConnectionId = this.GetPrimaryKey();
            _connection = new Connection(State, this);
            await base.OnActivateAsync();
        }
    }
}