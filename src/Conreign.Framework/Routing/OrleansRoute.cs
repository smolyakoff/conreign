using System;
using System.Linq;
using System.Reflection;

namespace Conreign.Framework.Routing
{
    public class OrleansRoute : IRoute
    {
        private readonly Lazy<RouteType> _type; 

        internal OrleansRoute(string key, MethodInfo method)
        {
            _type = new Lazy<RouteType>(() => new RouteType(method.GetParameters().First().ParameterType, method.ReturnType.GetGenericArguments().First()));
            Key = key;
            Method = method;
        }

        public string Key { get; }

        public RouteType Type => _type.Value;

        public MethodInfo Method { get; }

        public override string ToString()
        {
            // ReSharper disable once PossibleNullReferenceException
            return $"ORLEANS [{Key}] - {Method.DeclaringType.Name}({Type.RequestType.Name}): ${Type.ResponseType.Name}";
        }
    }
}
