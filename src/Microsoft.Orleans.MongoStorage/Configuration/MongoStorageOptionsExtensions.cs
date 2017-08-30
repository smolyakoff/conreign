using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Orleans.Providers;

namespace Microsoft.Orleans.MongoStorage.Configuration
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
            var connectionString = config.GetProperty(nameof(options.ConnectionString), MongoStorageOptions.DefaultConnectionString);
            var collectionNamePrefix = config.GetProperty(nameof(options.CollectionNamePrefix), null);
            var grainAssembliesCsv = config.GetProperty(nameof(options.GrainAssemblies), string.Empty);
            options.ConnectionString = connectionString;
            options.CollectionNamePrefix = collectionNamePrefix;
            options.GrainAssemblies = grainAssembliesCsv
                .Split(new[] {';'}, StringSplitOptions.RemoveEmptyEntries)
                .Select(x => x.Trim())
                .Select(name =>
                {
                    try
                    {
                        return Assembly.Load(name);
                    }
                    catch (Exception ex)
                    {
                        throw new InvalidOperationException($"Failed to load grain assembly {name}. {ex.Message}", ex);
                    }
                })
                .ToList();
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
                [nameof(options.GrainAssemblies)] = string.Join(";", options.GrainAssemblies.Select(a => a.FullName))
            };
        }
    }
}
