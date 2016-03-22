using System;

namespace Conreign.Framework.Contracts.Routing
{
    public sealed class StreamKey : IEquatable<StreamKey>
    {
        public StreamKey(Guid id, string ns)
        {
            Id = id;
            Namespace = ns;
        }

        public string Namespace { get; }

        public Guid Id { get; }

        public bool Equals(StreamKey other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return string.Equals(Namespace, other.Namespace) && Id.Equals(other.Id);
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
            return Equals((StreamKey) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((Namespace?.GetHashCode() ?? 0)*397) ^ Id.GetHashCode();
            }
        }

        public static bool operator ==(StreamKey left, StreamKey right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(StreamKey left, StreamKey right)
        {
            return !Equals(left, right);
        }
    }
}