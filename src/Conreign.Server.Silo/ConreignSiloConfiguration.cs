using System;
using Conreign.Utility.Configuration;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime.Configuration;
using Serilog.Events;

namespace Conreign.Server.Silo
{
    public class ConreignSiloConfiguration
    {
        public const string DefaultEnvironment = "local";

        public ConreignSiloConfiguration()
        {
            LivenessStorageType = GlobalConfiguration.LivenessProviderType.MembershipTableGrain;
            RemindersStorageType = GlobalConfiguration.ReminderServiceProviderType.ReminderTableGrain;
            DataStorageType = StorageType.InMemory;
            MinimumLogLevel = LogEventLevel.Information;
            InstanceId = System.Environment.MachineName;
        }

        public string Environment { get; set; }
        public string ClusterId { get; set; }
        public string InstanceId { get; set; }
        public LogEventLevel MinimumLogLevel { get; set; }
        public GlobalConfiguration.LivenessProviderType LivenessStorageType { get; set; }
        public GlobalConfiguration.ReminderServiceProviderType RemindersStorageType { get; set; }
        public StorageType DataStorageType { get; set; }
        public string SystemStorageConnectionString { get; set; }
        public string DataStorageConnectionString { get; set; }

        public static ConreignSiloConfiguration Load(string baseDirectory, string environment, string[] args = null)
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                throw new ArgumentException("Base directory cannot be null or empty.", nameof(baseDirectory));
            }
            if (string.IsNullOrEmpty(environment))
            {
                throw new ArgumentException("Environment cannot be null or empty.", nameof(environment));
            }
            var options = new ConreignSiloConfiguration
            {
                Environment = environment
            };
            var builder = new ConfigurationBuilder();
            builder.SetBasePath(baseDirectory)
                .AddJsonFile("silo.config.json", false)
                .AddJsonFile($"silo.{environment}.config.json", true)
                .AddJsonFile("silo.secrets.json", true)
                .AddJsonFile($"silo.{environment}.secrets.json", true)
                .AddCommandLine(args ?? Array.Empty<string>())
                .AddCloudConfiguration(c => c.UseKeys(
                    "SystemStorageConnectionString",
                    "DataStorageConnectionString",
                    "ElasticSearchUri"));
            var configRoot = builder.Build();
            configRoot.Bind(options);
            options.Environment = environment;
            return options;
        }

        public static ConreignSiloConfiguration Load(string baseDirectory, string[] args)
        {
            args = args ?? Array.Empty<string>();
            var envConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            var environment = envConfig.GetValue("Environment", DefaultEnvironment);
            return Load(baseDirectory, environment, args);
        }
    }
}