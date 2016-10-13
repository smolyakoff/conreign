using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Connected : ISystemEvent
    {
        public Connected(Guid connectionId, IPublisher<ISystemEvent> connection)
        {
            ConnectionId = connectionId;
            Connection = connection;
        }

        public Guid ConnectionId { get; }
        public IPublisher<ISystemEvent> Connection { get; }
    }
}