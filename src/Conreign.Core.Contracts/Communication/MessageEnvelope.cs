using System;
using System.Collections.Generic;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Communication
{
    [Serializable]
    [Immutable]
    public class MessageEnvelope
    {
        public MessageEnvelope(object message, params string[] connectionIds)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (connectionIds == null)
            {
                throw new ArgumentNullException(nameof(connectionIds));
            }
            ConnectionIds = new HashSet<string>(connectionIds);
            Message = message;
        }

        public HashSet<string> ConnectionIds { get; }
        public object Message { get; }
    }
}
