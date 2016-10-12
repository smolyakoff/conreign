using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Connected : ISystemEvent
    {
        public Connected(Guid connectionId, ISystemPublisher connection)
        {
            ConnectionId = connectionId;
            Connection = connection;
        }

        public Guid ConnectionId { get; }
        public ISystemPublisher Connection { get; }
    }
}