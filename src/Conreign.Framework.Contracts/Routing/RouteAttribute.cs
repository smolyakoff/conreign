using System;

namespace Conreign.Framework.Contracts.Routing
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class RouteAttribute: Attribute
    {
        public RouteAttribute(Type requestType, Type responseType, string key)
        {
            if (requestType == null)
            {
                throw new ArgumentNullException(nameof(requestType));
            }
            if (responseType == null)
            {
                throw new ArgumentNullException(nameof(responseType));
            }
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }
            RequestType = requestType;
            ResponseType = responseType;
            Key = key;
        }

        public string Key { get; private set; }

        public Type RequestType { get; }

        public Type ResponseType { get; }
    }
}
