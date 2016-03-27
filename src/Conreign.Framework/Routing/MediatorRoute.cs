using System;

namespace Conreign.Framework.Routing
{
    public class MediatorRoute : IRoute
    {
        internal MediatorRoute(string key, Type requestType, Type responseType)
        {
            Key = key;
            Type = new RouteType(requestType, responseType);
        }

        public string Key { get; }

        public RouteType Type { get; }
    }
}
