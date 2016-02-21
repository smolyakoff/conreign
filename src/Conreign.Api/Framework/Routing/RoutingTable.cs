using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Api.Framework.Routing
{
    public class RoutingTable
    {
        private readonly Assembly _grainInterfacesAssembly;

        private Dictionary<string, Route> _routes;

        private readonly object _locker = new object();

        public RoutingTable(Assembly grainInterfacesAssembly)
        {
            if (grainInterfacesAssembly == null)
            {
                throw new ArgumentNullException(nameof(grainInterfacesAssembly));
            }
            _grainInterfacesAssembly = grainInterfacesAssembly;
            _routes = null;
        }

        public Route Match(GenericAction action)
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
                var actionTypes = types.Where(IsGrainAction);
                _routes = actionTypes
                    .Select(t => new { Key = GetActionKey(t), ActionType = t, Method = GetMethod(t) })
                    .Where(x => x.Method != null)
                    .ToDictionary(x => x.Key, x => new Route(x.ActionType, x.Method), StringComparer.OrdinalIgnoreCase);
                return _routes;
            }
        }

        private static bool IsGrainAction(Type type)
        {
            return type.IsClass && !type.IsAbstract && GetGrainActionInterface(type) != null;
        }

        private static string GetActionKey(Type type)
        {
            var regex = new Regex("Action$", RegexOptions.Compiled);
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
