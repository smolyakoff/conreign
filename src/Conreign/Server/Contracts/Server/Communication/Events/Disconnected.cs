using Conreign.Server.Contracts.Shared.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Server.Communication.Events;

[Serializable]
[Immutable]
public class Disconnected : IServerEvent
{
    public Disconnected(string connectionId)
    {
        ConnectionId = connectionId;
    }

    public string ConnectionId { get; }
}