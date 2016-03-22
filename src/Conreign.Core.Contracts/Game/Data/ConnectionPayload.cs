using System;

namespace Conreign.Core.Contracts.Game.Data
{
    public class ConnectionPayload
    {
        public ConnectionPayload(string connectionId)
        {
            if (string.IsNullOrEmpty(connectionId))
            {
                throw new ArgumentNullException(nameof(connectionId));
            }
            ConnectionId = connectionId;
        }

        public string ConnectionId { get; }
    }
}