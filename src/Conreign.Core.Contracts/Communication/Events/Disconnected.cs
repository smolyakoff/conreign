using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Disconnected : IServerEvent
    {
        public Disconnected(Guid connectionId)
        {
            ConnectionId = connectionId;
        }

        public Guid ConnectionId { get;  }
    }
}