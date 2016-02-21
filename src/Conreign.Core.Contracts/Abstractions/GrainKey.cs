using System;
using Orleans;

namespace Conreign.Core.Contracts.Abstractions
{
    public sealed class GrainKey<TGrain> : IEquatable<GrainKey<TGrain>> where TGrain : IGrain
    {
        internal GrainKey(string key) : this((object)key)
        {
        }

        internal GrainKey(long key) : this((object)key)
        {
        }

        internal GrainKey(Guid key) : this((object)key)
        {
        }

        private GrainKey(object key)
        {
            if (key == null)
            {
                throw new ArgumentNullException(nameof(key));
            }
            EnsureKeyIsValid(key);
            GrainType = typeof(TGrain);
            Key = key;
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

        private static void EnsureKeyIsValid(object key)
        {
            var keyType = key.GetType();
            Type grainInterface;
            if (keyType == typeof(string))
            {
                grainInterface = typeof (IGrainWithStringKey);
            }
            else if (keyType == typeof (Guid))
            {
                grainInterface = typeof (IGrainWithGuidKey);
            }
            else if (keyType == typeof (long))
            {
                grainInterface = typeof (IGrainWithIntegerKey);
            }
            else
            {
                throw new ArgumentException($"Invalid grain key type: {keyType.Name}", nameof(key));
            }
            if (!grainInterface.IsAssignableFrom(typeof (TGrain)))
            {
                throw new ArgumentException($"Key doesn't match grain type: {typeof(TGrain).Name}");
            }
        }
    }
}