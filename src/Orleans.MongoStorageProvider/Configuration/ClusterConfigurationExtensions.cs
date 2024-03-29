﻿using System;
using Orleans.Runtime.Configuration;

namespace Orleans.MongoStorageProvider.Configuration
{
    public static class ClusterConfigurationExtensions
    {
        public static void AddMongoDbStorageProvider(
            this ClusterConfiguration config,
            string providerName = "Default",
            MongoStorageOptions options = null)
        {
            if (config == null)
            {
                throw new ArgumentNullException(nameof(config));
            }
            if (string.IsNullOrWhiteSpace(providerName))
            {
                throw new ArgumentException("Provider name cannot be null or whitespace.", nameof(providerName));
            }
            options = options ?? new MongoStorageOptions();
            config.Globals.RegisterStorageProvider<MongoStorageProvider>(providerName, options.ToDictionary());
        }
    }
}