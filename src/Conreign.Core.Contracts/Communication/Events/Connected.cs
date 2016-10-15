using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Connected : IServerEvent
    {
        public Connected(Guid connectionId, IPublisher<IServerEvent> connection)
        {
            ConnectionId = connectionId;
            Connection = connection;
        }

        public Guid ConnectionId { get; }
        public IPublisher<IServerEvent> Connection { get; }
    }
}