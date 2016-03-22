using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Abstractions.Actions;
using Serilog;

namespace Conreign.Api.Framework.Routing
{
    public class RoutingTable
    {
        private readonly Assembly _grainInterfacesAssembly;

        private readonly ILogger _logger;

        private Dictionary<string, Route> _routes;

        private readonly object _locker = new object();

        public RoutingTable(Assembly grainInterfacesAssembly)
        {
            if (grainInterfacesAssembly == null)
            {
                throw new ArgumentNullException(nameof(grainInterfacesAssembly));
            }
            _logger = Log.ForContext(typeof (RoutingTable));
            _grainInterfacesAssembly = grainInterfacesAssembly;
            _routes = null;
        }

        public Route Match(HttpAction action)
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
                var types = _grainInterfacesAssembly.DefinedTypes;
                var actionTypes = types.Where(IsPublicGrainAction);
                _routes = actionTypes
                    .Select(t => new { Key = GetActionKey(t), ActionType = t, Method = GetMethod(t) })
                    .Where(x => x.Method != null)
                    .ToDictionary(x => x.Key, x => new Route(x.ActionType, x.Method), StringComparer.OrdinalIgnoreCase);
                _logger.Information("Registered routes: {Routes}", _routes);
                return _routes;
            }
        }

        private static bool IsPublicGrainAction(Type type)
        {
            return type.IsClass && 
                !type.IsAbstract && 
                GetGrainActionInterface(type) != null &&
                !IsInternalAction(type);
        }

        private static bool IsInternalAction(Type type)
        {
            var action = type.GetCustomAttribute<ActionAttribute>();
            return action != null && action.Internal;
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
