using System;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication.Events
{
    [Serializable]
    [Immutable]
    public class Disconnected : ISystemEvent
    {
        public Disconnected(Guid connectionId)
        {
            ConnectionId = connectionId;
        }

        public Guid ConnectionId { get;  }
    }
}