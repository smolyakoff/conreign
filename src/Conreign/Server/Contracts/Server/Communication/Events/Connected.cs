using Conreign.Server.Contracts.Shared.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Server.Communication.Events;

[Serializable]
[Immutable]
public class Connected : IServerEvent
{
    public Connected(string connectionId, string topicId)
    {
        ConnectionId = connectionId;
        TopicId = topicId;
    }

    public string ConnectionId { get; }
    public string TopicId { get; }
}