using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Connected : IServerEvent
    {
        public Connected(Guid connectionId, string topicId)
        {
            ConnectionId = connectionId;
            TopicId = topicId;
        }

        public Guid ConnectionId { get; }
        public string TopicId { get; }
    }
}