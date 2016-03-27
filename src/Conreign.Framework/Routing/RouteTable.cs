using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Conreign.Framework.Contracts.Routing;
using MediatR;
using Orleans;
using Serilog;

namespace Conreign.Framework.Routing
{
    public class RouteTableOptions
    {
        public Assembly[] Assemblies { get; set; }

        public IRouteKeyConvention KeyConvention { get; set; }
    }

    public class RouteTable
    {
        private class MessageDescriptor : IRouteDescriptor
        {
            public string Name { get; set; }
            public RouteType RouteType { get; set; }
        }

        private class GrainMethodDescriptor : IRouteDescriptor
        {
            public string Name { get; set; }
            public RouteType RouteType { get; set; }
            public MethodInfo Method { get; set; } 
        }

        private interface IRouteDescriptor
        {
            RouteType RouteType { get; } 
        }

        private static readonly ILogger Logger;

        static RouteTable()
        {
            Logger = Log.ForContext<RouteTable>();
        }

        private readonly Assembly _grainInterfacesAssembly;

        private readonly object _locker = new object();

        private Dictionary<string, Route> _routes;

        public static RouteTable Build(RouteTableOptions options)
        {
            var types = options.Assemblies.SelectMany(x => x.ExportedTypes).ToList();
            var messageDescriptors = types.SelectMany(x => GetMediatorRoutes(x, options.KeyConvention)).ToList();

            var methodDescriptors = types.SelectMany(x => GetOrleansRoutes(x, messageDescriptors)).ToList();

            var orleansRoutes = methodDescriptors.Select(x => new OrleansRoute(x.Name, x.Method));
            var mediatorRoutes = messageDescriptors
                .Select(x => new MediatorRoute(x.Name, x.RouteType.RequestType, x.RouteType.ResponseType));


            var messageByRouteKey = messageDescriptors.ToDictionary(x => x.RouteType);
            
            var routeLookup = new Dictionary<string, List<IRoute>>();
            foreach (var method in methodDescriptors)
            {
                var name = method.Name ?? (messageByRouteKey.ContainsKey(method.RouteType) ? messageByRouteKey[method.RouteType].Name : null);
                if (string.IsNullOrEmpty(name))
                {
                    // ReSharper disable once PossibleNullReferenceException
                    Logger.Warning($"No route information for grain method: {method.Method.DeclaringType.Name}({method.Method.Name})");
                    continue;
                }
                var list = routeLookup.ContainsKey(name) ? routeLookup[name] : new List<IRoute>();
                routeLookup[name] = list;
                list.Add(new OrleansRoute(method.Name, method.Method));
            }
            foreach (var messageDescriptor in messageDescriptors.Where(m => !routeLookup.ContainsKey(m.Name)))
            {
                var list = routeLookup[messageDescriptor.Name];
                list.Add(new MediatorRoute(messageDescriptor.Name, messageDescriptor.RouteType.RequestType, messageDescriptor.RouteType.ResponseType));
            }
        }

        private static IEnumerable<Type> GetRequestInterfaces(Type type)
        {
            return type.GetInterfaces()
                .Where(t => t.IsGenericType)
                .Where(t => t.GetGenericTypeDefinition() == typeof (IAsyncRequest<>));
        }

        private static IEnumerable<OrleansRoute> GetOrleansRoutes(Type type, ILookup<RouteType, MediatorRoute> mediatorRoutesLookup)
        {
            if (!type.IsInterface || !typeof(IGrain).IsAssignableFrom(type))
            {
                return Enumerable.Empty<OrleansRoute>();
            }
            var routes = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                .Select(m => new
                {
                    Method = m,
                    Parameters = m.GetParameters(),
                    m.ReturnType
                })
                .Where(m => m.Parameters.Length == 1)
                .Where(m => m.ReturnType.IsGenericType && m.ReturnType.GetGenericTypeDefinition() == typeof (Task<>))
                .Select(m => new
                {
                    m.Method,
                    RouteKey = new RouteType(m.Parameters[0].ParameterType, m.ReturnType)
                })
                .SelectMany(m =>
                {
                    if (!mediatorRoutesLookup.Contains(m.RouteKey))
                    {
                        var route = 
                        return Enumerable.Empty<OrleansRoute>();
                    }
                    return messageLookups[m.RouteKey].Select(x => new GrainMethodDescriptor
                    {
                        Method = m.Method,
                        Name = x.Name,
                        RouteType = x.RouteType
                    });
                });
            return routes;
        }

        private static IEnumerable<MediatorRoute> GetMediatorRoutes(Type type, IRouteKeyConvention convention)
        {
            var routes = GetRequestInterfaces(type)
                .Select(t => new
                {
                    RequestType = type,
                    ResponseType = t.GetGenericArguments().First(),
                    OtherResponseTypes = GetRequestInterfaces(type).Except(Enumerable.Repeat(t, 1)).Select(x => x.GetGenericArguments()[0])
                })
                .Select(d => new MediatorRoute(
                    convention.GetKey(d.RequestType, d.ResponseType, d.OtherResponseTypes),
                    d.RequestType,
                    d.ResponseType
                ))
                .ToList();
            if (routes.Count == 0)
            {
                return Enumerable.Empty<MediatorRoute>();
            }
            var attributeRoutes = type
                .GetCustomAttributes<RouteAttribute>()
                .Select(a => new MediatorRoute(a.Key, a.RequestType, a.ResponseType))
                .ToList();
            var attributeRoutesLookup = attributeRoutes.ToLookup(d => d.Type);
            //var conflicts = attributesLookup.Where(g => g.Count() > 1).ToList();
            //if (conflicts.Count > 0)
            //{
            //    var messages = conflicts.Select(c => $"{c.Key.RequestType.Name}-{c.Key.ResponseType.Name}:{string.Join(",", c.Select(x => x.Name))}");
            //    var builder = new StringBuilder();
            //    builder.AppendLine("Ambiguous route attribute configuration: ");
            //    foreach (var message in messages)
            //    {
            //        builder.AppendLine(message);
            //    throw new InvalidOperationException(builder.ToString());
            //}
            //var names = attributesLookup.ToDictionary(x => x.Key, x => x.First().Name);
            return attributeRoutes.Concat(routes.Where(d => !attributeRoutesLookup.Contains(d.Type)));
        }

        private RouteTable(Assembly grainInterfacesAssembly)
            //    }
        {
            if (grainInterfacesAssembly == null)
            {
                throw new ArgumentNullException(nameof(grainInterfacesAssembly));
            }
            Logger = Log.ForContext(typeof (RouteTable));
            _grainInterfacesAssembly = grainInterfacesAssembly;
            _routes = null;
        }

        public IRoute Match(string name)
        {
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
            var routeAttributes = type.GetCustomAttributes<RouteAttribute>().ToDictionary(x => x.Key, x => x);
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