using System;

namespace Conreign.Framework.Contracts.Routing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RouteAttribute: Attribute
    {
        public RouteAttribute(Type requestType, Type responseType, string name)
        {
            if (requestType == null)
            {
                throw new ArgumentNullException(nameof(requestType));
            }
            if (responseType == null)
            {
                throw new ArgumentNullException(nameof(responseType));
            }
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }
            RequestType = requestType;
            ResponseType = responseType;
            Name = name;
        }

        public string Name { get; private set; }

        public Type RequestType { get; }

        public Type ResponseType { get; }
    }
}
