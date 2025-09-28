using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Server.Presence;
using Conreign.Server.Core.Communication;
using Orleans;

namespace Conreign.Server.Core.Presence;

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
        var topic = new Topic(GetStreamProvider(StreamConstants.ProviderName), id);
        return Task.FromResult((ITopic)topic);
    }

    public override async Task OnActivateAsync()
    {
        State.ConnectionId = this.GetPrimaryKeyString();
        _connection = new Connection(State, this);
        await base.OnActivateAsync();
    }
}