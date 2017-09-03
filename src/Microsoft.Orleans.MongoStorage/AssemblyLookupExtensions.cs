using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orleans;

namespace Microsoft.Orleans.MongoStorage
{
    internal static class AssemblyLookupExtensions
    {
        public static IEnumerable<Type> GetGrainStateTypes(this Assembly assembly)
        {
            return assembly
                .GetExportedTypes()
                .Where(IsGrainWithStateType)
                .Select(GetGrainStateType);
        }

        private static bool IsGrainWithStateType(Type type)
        {
            return type.BaseType != null &&
                   type.BaseType.IsGenericType &&
                   type.BaseType.GetGenericTypeDefinition() == typeof(Grain<>);
        }

        private static Type GetGrainStateType(Type grainType)
        {
            // ReSharper disable once PossibleNullReferenceException
            var grainStateType = grainType.BaseType.GetGenericArguments().First();
            return grainStateType;
        }
    }
}