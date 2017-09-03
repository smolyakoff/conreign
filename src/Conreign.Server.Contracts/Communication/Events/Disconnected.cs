using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Disconnected : IServerEvent
    {
        public Disconnected(Guid connectionId)
        {
            ConnectionId = connectionId;
        }

        public Guid ConnectionId { get; }
    }
}