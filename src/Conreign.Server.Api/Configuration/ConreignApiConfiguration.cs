using System;
using Conreign.Utility.Configuration;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime.Configuration;
using Serilog.Events;

namespace Conreign.Server.Api.Configuration
{
    public class ConreignApiConfiguration
    {
        public const string DefaultEnvironment = "local";

        private ConreignApiConfiguration(string environment)
        {
            Environment = environment;
            MinimumLogLevel = LogEventLevel.Information;
            SystemStorageType = ClientConfiguration.GatewayProviderType.Config;
            ClusterId = environment;
            InstanceId = System.Environment.MachineName;
        }

        public string Environment { get; }
        public bool IsLocalEnvironment => Environment == DefaultEnvironment;
        public string ClusterId { get; set; }
        public string InstanceId { get; set; }
        public int Port { get; set; } = 3000;
        public LogEventLevel MinimumLogLevel { get; set; }
        public string ElasticSearchUri { get; set; }
        public ClientConfiguration.GatewayProviderType SystemStorageType { get; set; }
        public string SystemStorageConnectionString { get; set; }

        public static ConreignApiConfiguration Load(string baseDirectory, string[] args = null)
        {
            args = args ?? Array.Empty<string>();
            var envConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            var environment = envConfig.GetValue("Environment", DefaultEnvironment);
            return Load(baseDirectory, environment, args);
        }

        public static ConreignApiConfiguration Load(string baseDirectory, string environment, string[] args = null)
        {
            if (string.IsNullOrEmpty(baseDirectory))
            {
                throw new ArgumentException("Base directory cannot be null or empty.", nameof(baseDirectory));
            }
            if (string.IsNullOrEmpty(environment))
            {
                throw new ArgumentException("Environment cannot be null or empty.", nameof(environment));
            }
            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(baseDirectory)
                .AddJsonFile("api.config.json", false)
                .AddJsonFile("api.secrets.json", true)
                .AddJsonFile($"api.{environment}.config.json", true)
                .AddJsonFile($"api.{environment}.secrets.json", true)
                .AddCommandLine(args ?? Array.Empty<string>())
                .AddCloudConfiguration(c => c.UseKeys(
                    "MinimumLogLevel",
                    "SystemStorageConnectionString",
                    "ElasticSearchUri"
                ));
            var config = new ConreignApiConfiguration(environment);
            var configRoot = builder.Build();
            configRoot.Bind(config);
            return config;
        }
    }
}