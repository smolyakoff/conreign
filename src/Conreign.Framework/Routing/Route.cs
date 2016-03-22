using System;
using System.Collections.Immutable;
using System.Reflection;

namespace Conreign.Framework.Routing
{
    public class Route
    {
        internal Route(Type requestType, Type responseType, MethodInfo method)
        {
            if (requestType == null)
            {
                throw new ArgumentNullException(nameof(requestType));
            }
            if (responseType == null)
            {
                throw new ArgumentNullException(nameof(responseType));
            }
            if (method == null)
            {
                throw new ArgumentNullException(nameof(method));
            }
            RequestType = requestType;
        }

        public string Name { get; }

        public Type RequestType { get; }

        public Type ResponseType { get; }

        public ImmutableHashSet<Type> HandlerTypes { get; }

        public override string ToString()
        {
            return $"{RequestType.Name}";
        }
    }
}