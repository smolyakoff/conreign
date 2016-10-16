using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Connected : IServerEvent
    {
        public Connected(Guid connectionId, IPublisher<Disconnected> connection)
        {
            ConnectionId = connectionId;
            Connection = connection;
        }

        public Guid ConnectionId { get; }
        public IPublisher<Disconnected> Connection { get; }
    }
}