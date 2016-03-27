using System;
using System.Linq;
using Orleans;

namespace Conreign.Framework.Routing
{
    internal static class GrainFactoryExtensions
    {
        public static IGrain GetGrain(this IGrainFactory factory, Type grainType, object key)
        {
            var method = factory.GetType()
                .GetMethods()
                .Where(m => m.Name == "GetGrain")
                .First(m => m.GetParameters().First().ParameterType == key.GetType())
                .MakeGenericMethod(grainType);
            return method.Invoke(factory, new[] {key, null}) as IGrain;
        }
    }
}