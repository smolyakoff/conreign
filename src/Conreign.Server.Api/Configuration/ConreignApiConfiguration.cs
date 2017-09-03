using Conreign.Utility.Configuration;
using Microsoft.Extensions.Configuration;
using Orleans.Runtime.Configuration;
using Serilog.Events;

namespace Conreign.Server.Api.Configuration
{
    public class ConreignApiConfiguration
    {
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

        public static ConreignApiConfiguration Load(string baseDirectory = null, string environment = "development")
        {
            baseDirectory = string.IsNullOrEmpty(baseDirectory) ? System.Environment.CurrentDirectory : baseDirectory;
            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(baseDirectory)
                .AddJsonFile("api.config.json", false)
                .AddJsonFile("api.secrets.json", true)
                .AddJsonFile($"api.{environment}.config.json", true)
                .AddJsonFile($"api.{environment}.secrets.json", true)
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
            return config;
        }
    }
}