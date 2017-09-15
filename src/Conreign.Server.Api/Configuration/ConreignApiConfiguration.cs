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

        public ConreignApiConfiguration()
        {
            Path = string.Empty;
            MinimumLogLevel = LogEventLevel.Information;
            SystemStorageType = ClientConfiguration.GatewayProviderType.Config;
        }

        public string Environment { get; set; }
        public string Path { get; set; }
        public int Port { get; set; } = 3000;
        public LogEventLevel MinimumLogLevel { get; set; }
        public ClientConfiguration.GatewayProviderType SystemStorageType { get; set; }
        public string SystemStorageConnectionString { get; set; }
        public string ElasticSearchUri { get; set; }

        public static ConreignApiConfiguration Load(string baseDirectory, string[] args)
        {
            args = args ?? Array.Empty<string>();
            var envConfig = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            var environment = envConfig.GetValue("Environment", DefaultEnvironment);
            return Load(baseDirectory, environment, args);
        }

        public static ConreignApiConfiguration Load(
            string baseDirectory = null, 
            string environment = DefaultEnvironment, 
            string[] args = null)
        {
            baseDirectory = string.IsNullOrEmpty(baseDirectory) ? System.Environment.CurrentDirectory : baseDirectory;
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
            var config = new ConreignApiConfiguration
            {
                Environment = environment
            };
            var configRoot = builder.Build();
            configRoot.Bind(config);
            config.Environment = environment;
            return config;
        }
    }
}