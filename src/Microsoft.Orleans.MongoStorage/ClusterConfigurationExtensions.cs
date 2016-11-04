using System;
using System.Collections.Generic;
using Orleans.Runtime.Configuration;

namespace Microsoft.Orleans.Storage
{
    public static class ClusterConfigurationExtensions
    {
        public static void AddMongoDbStorageProvider(this ClusterConfiguration config, string providerName = "Default", string connectionString = null) 
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be null or whitespace.", nameof(providerName));
            }
            config.Globals.RegisterStorageProvider<MongoStorage>(providerName, new Dictionary<string, string>
            {
                ["ConnectionString"] = connectionString
            });
        }
    }
}
