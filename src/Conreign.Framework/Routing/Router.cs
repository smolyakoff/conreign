using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Conreign.Framework.Contracts.Data;
using Conreign.Framework.Contracts.Routing;
using MediatR;
using Serilog;

namespace Conreign.Framework.Routing
{
    public interface IRouteNamingConvention
    {
        string GetName(Type requestType, Type responseType, IEnumerable<Type> otherResponseTypes);
    }

    public class Router
    {


        private readonly Assembly _grainInterfacesAssembly;

        private readonly object _locker = new object();

        private readonly ILogger _logger;

        private Dictionary<string, Route> _routes;

        public static Router Build(params Assembly[] assemblies)
        {
            var types = assemblies.SelectMany(a => a.ExportedTypes);

        }

        private Router(Assembly grainInterfacesAssembly)
        {
            if (grainInterfacesAssembly == null)
            {
                throw new ArgumentNullException(nameof(grainInterfacesAssembly));
            }
            _logger = Log.ForContext(typeof (Router));
            _grainInterfacesAssembly = grainInterfacesAssembly;
            _routes = null;
        }

        public Route Match(Request action)
        {
            if (action == null)
            {
                throw new ArgumentNullException(nameof(action));
            }
            var routes = Build();
            return routes.ContainsKey(action.Type)
                ? routes[action.Type]
                : null;
        }

        private Dictionary<string, Route> Build()
        {
            if (_routes != null)
            {
                return _routes;
            }
            lock (_locker)
            {
                
            }
        }

        private static IEnumerable<Route> GetRoutes(Type type)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return Enumerable.Empty<Route>();
            }
            var interfaces = type.GetInterfaces()
                .Where(t => t.IsGenericType && t.GetGenericTypeDefinition() == typeof (IAsyncRequest<>))
                .ToList();
            if (interfaces.Count == 0)
            {
                return Enumerable.Empty<Route>();
            }
            var routeAttributes = type.GetCustomAttributes<RouteAttribute>().ToDictionary(x => x.Name, x => x);
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IAsyncRequest<>);
        }

        private static string GetRouteName(Type requestType, Type responseType, bool single)
        {
            var regex = new Regex("Action|M$");
        }

        private static bool IsPublicGrainAction(Type type)
        {
            return type.IsClass &&
                   !type.IsAbstract &&
                   GetGrainActionInterface(type) != null;
        }

        private static string GetActionKey(Type type)
        {
            var regex = new Regex("Action|Query|Command$", RegexOptions.Compiled);
            return type.GetCustomAttribute<ActionAttribute>()?.Type ?? regex.Replace(type.Name, string.Empty);
        }

        private static MethodInfo GetMethod(Type type)
        {
            var grainAction = GetGrainActionInterface(type);
            var grainType = grainAction.GetGenericArguments()[0];
            return grainType.GetMethods()
                .FirstOrDefault(m => m.GetParameters().Any(p => type.IsAssignableFrom(p.ParameterType)));
        }

        private static Type GetGrainActionInterface(Type type)
        {
            return type.GetInterfaces()
                .Where(t => t.IsConstructedGenericType)
                .FirstOrDefault(t => t.GetGenericTypeDefinition() == typeof (IGrainAction<>));
        }
    }
}