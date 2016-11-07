using System;
using Conreign.Utility.Configuration;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime.Configuration;

namespace Conreign.Cluster
{
    public class ConreignSiloConfiguration
    {
        public ConreignSiloConfiguration()
        {
            LivenessStorageType = GlobalConfiguration.LivenessProviderType.MembershipTableGrain;
            RemindersStorageType = GlobalConfiguration.ReminderServiceProviderType.ReminderTableGrain;
            DataStorageType = StorageType.InMemory;
        }

        public string Environment { get; set; }
        public GlobalConfiguration.LivenessProviderType LivenessStorageType { get; set; }
        public GlobalConfiguration.ReminderServiceProviderType RemindersStorageType { get; set; }
        public StorageType DataStorageType { get; set; }
        public string SystemStorageConnectionString { get; set; }
        public string DataStorageConnectionString { get; set; }

        public static ConreignSiloConfiguration Load(string baseDirectory, string environment = "development")
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                throw new ArgumentException("Base directory cannot be null or empty.", nameof(baseDirectory));
            }
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(baseDirectory)
                .AddJsonFile("silo.config.json", false)
                .AddJsonFile($"silo.{environment}.config.json", true)
                .AddJsonFile("silo.secrets.json", true)
                .AddJsonFile($"silo.{environment}.secrets.json", true)
                .AddCloudConfiguration(c => c.UseKeys(
                    "SystemStorageConnectionString",
                    "DataStorageConnectionString",
                    "ElasticSearchUri"));
            var configRoot = builder.Build();
            var options = new ConreignSiloConfiguration
            {
                Environment = environment
            };
            configRoot.Bind(options);
            return options;
        }
    }
}
