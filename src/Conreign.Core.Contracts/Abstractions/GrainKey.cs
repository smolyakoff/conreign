using System;
using Orleans;

namespace Conreign.Core.Contracts.Abstractions
{
    public sealed class GrainKey<TGrain> : IEquatable<GrainKey<TGrain>> where TGrain : IGrain
    {
        internal GrainKey(object key)
        {
            Key = key;
            GrainType = typeof (TGrain);
        }

        public object Key { get; }

        public Type GrainType { get; }

        public bool Equals(GrainKey<TGrain> other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return Key.Equals(other.Key) && GrainType == other.GrainType;
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
            return obj.GetType() == GetType() && Equals((GrainKey<TGrain>) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Key.GetHashCode()*397) ^ GrainType.GetHashCode();
            }
        }

        public static bool operator ==(GrainKey<TGrain> left, GrainKey<TGrain> right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(GrainKey<TGrain> left, GrainKey<TGrain> right)
        {
            return !Equals(left, right);
        }
    }
}