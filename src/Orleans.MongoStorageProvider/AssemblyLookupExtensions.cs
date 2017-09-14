using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orleans.MongoStorageProvider.Driver;

namespace Orleans.MongoStorageProvider
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

        public static IEnumerable<IMongoDriverBootstrap> GetMongoDriverBootstraps(this Assembly assembly)
        {
            return assembly
                .GetTypes()
                .Where(IsMongoDriverBootstrapType)
                .Select(Activator.CreateInstance)
                .Cast<IMongoDriverBootstrap>();
        }

        private static bool IsMongoDriverBootstrapType(Type type)
        {
            return typeof(IMongoDriverBootstrap).IsAssignableFrom(type) &&
                type.GetConstructor(Type.EmptyTypes) != null;
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