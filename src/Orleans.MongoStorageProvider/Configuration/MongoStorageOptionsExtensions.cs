using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orleans.Providers;

namespace Orleans.MongoStorageProvider.Configuration
{
    internal static class MongoStorageOptionsExtensions
    {
        public static MongoStorageOptions DeserializeToMongoStorageOptions(this IProviderConfiguration config)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            var options = new MongoStorageOptions();
            var connectionString = config.GetProperty(nameof(options.ConnectionString),
                MongoStorageOptions.DefaultConnectionString);
            var collectionNamePrefix = config.GetProperty(nameof(options.CollectionNamePrefix), null);
            var grainAssemblies = config.GetProperty(nameof(options.GrainAssemblies), string.Empty);
            var bootstrapAssemblies = config.GetProperty(nameof(options.BootstrapAssemblies), string.Empty);
            options.ConnectionString = connectionString;
            options.CollectionNamePrefix = collectionNamePrefix;
            options.GrainAssemblies = LoadAssemblies(grainAssemblies);
            options.BootstrapAssemblies = LoadAssemblies(bootstrapAssemblies);
            return options;
        }

        public static Dictionary<string, string> ToDictionary(this MongoStorageOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            return new Dictionary<string, string>
            {
                [nameof(options.ConnectionString)] = options.ConnectionString,
                [nameof(options.CollectionNamePrefix)] = options.CollectionNamePrefix,
                [nameof(options.GrainAssemblies)] = string.Join(";", options.GrainAssemblies.Select(a => a.FullName)),
                [nameof(options.BootstrapAssemblies)] = string.Join(";", options.BootstrapAssemblies.Select(a => a.FullName))
            };
        }

        private static List<Assembly> LoadAssemblies(string assemblies)
        {
            return assemblies
                .Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Select(name =>
                {
                    try
                    {
                        return Assembly.Load(name);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Orleans MongoDB storage provider failed to load assembly {name}. {ex.Message}", ex);
                    }
                })
                .ToList();
        }
    }
}