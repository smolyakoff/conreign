using System;

namespace Conreign.Core.Game.Data
{
    public class ConnectionData : IEquatable<ConnectionData>
    {
        public ConnectionData(string connectionId)
        {
            ConnectionId = connectionId;
            ConnectedAt = DateTime.UtcNow;
        }

        public string ConnectionId { get; }

        public DateTime ConnectedAt { get; }

        public bool Equals(ConnectionData other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(ConnectionId, other.ConnectionId);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
            {
                return false;
            }
            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            if (obj.GetType() != GetType())
            {
                return false;
            }
            return Equals((ConnectionData) obj);
        }

        public override int GetHashCode()
        {
            return ConnectionId.GetHashCode();
        }
    }
}