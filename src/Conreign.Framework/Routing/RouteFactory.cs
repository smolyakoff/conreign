using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using MediatR;
using Orleans;

namespace Conreign.Framework.Routing
{
    internal class RouteFactory
    {
        public IEnumerable<IRoute> CreateRoutesForType(Type type)
        {
            if (!type.IsClass || type.IsAbstract)
            {
                return Enumerable.Empty<IRoute>();
            } 
            var interfaces = type.GetInterfaces().ToList();
            if (interfaces.Any(IsGrainInterface))
            {
                return CreateRoutesForGrainType(type);
            }
            if (interfaces.Any(IsHandlerInterface))
            {
                return CreateRoutesForHandlerType(type);
            }
            return Enumerable.Empty<IRoute>();
        }

        private IEnumerable<MediatorRoute> CreateRoutesForHandlerType(Type type)
        {
        }

        private IEnumerable<OrleansRoute> CreateRoutesForGrainType(Type type)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Public)
                .Where(IsFrameworkMethod)
                .ToList();
        }

        private static bool IsFrameworkMethod(MethodInfo methodInfo)
        {
            return methodInfo.GetParameters().Length == 1 && 
                methodInfo.ReturnType.IsGenericType &&
                methodInfo.ReturnType.GetGenericTypeDefinition() == typeof (Task<>);
        }

        private static bool IsHandlerInterface(Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof (IAsyncRequestHandler<,>);
        }

        private static bool IsGrainInterface(Type type)
        {
            return typeof (IGrain).IsAssignableFrom(type);
        }
    }
}
