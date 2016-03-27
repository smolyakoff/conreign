using System;

namespace Conreign.Framework.Routing
{
    public class RouteType : IEquatable<RouteType>
    {
        public RouteType(Type requestType, Type responseType)
        {
            RequestType = requestType;
            ResponseType = responseType;
        }

        public Type RequestType { get; }

        public Type ResponseType { get; }

        public bool Equals(RouteType other)
        {
            if (ReferenceEquals(null, other))
            {
                return false;
            }
            if (ReferenceEquals(this, other))
            {
                return true;
            }
            return RequestType == other.RequestType && ResponseType == other.ResponseType;
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
            return Equals((RouteType) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (RequestType.GetHashCode()*397) ^ ResponseType.GetHashCode();
            }
        }

        public static bool operator ==(RouteType left, RouteType right)
        {
            return Equals(left, right);
        }

        public static bool operator !=(RouteType left, RouteType right)
        {
            return !Equals(left, right);
        }
    }
}
