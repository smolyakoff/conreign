using System.IO;
using Microsoft.Extensions.Configuration;

namespace Conreign.Server.Host.Yams
{
    public class YamsServiceConfiguration
    {
        public const string DefaultEnvironment = "local";

        public string Environment { get; }
        public string ClusterId { get; set; }
        public string InstanceId { get; set; }
        public string InstanceUpdateDomain { get; set; }
        public string ApplicationInstallDirectory { get; set; }
        public int UpdatePeriodInSeconds { get; set; }
        public int RestartCount { get; set; }
        public string AzureStorageConnectionString { get; set; }

        private YamsServiceConfiguration(string environment = DefaultEnvironment)
        {
            Environment = environment;
            InstanceId = System.Environment.MachineName;
            InstanceUpdateDomain = InstanceId;
            ApplicationInstallDirectory = Path.Combine(System.Environment.CurrentDirectory, "Applications");
            UpdatePeriodInSeconds = 15;
            RestartCount = 3;
            AzureStorageConnectionString = "UseDevelopmentStorage=true";
        }

        public static YamsServiceConfiguration Load(string baseDirectory = null, string environment = DefaultEnvironment)
        {
            baseDirectory = string.IsNullOrEmpty(baseDirectory) ? System.Environment.CurrentDirectory : baseDirectory;
            var builder = new ConfigurationBuilder();
            builder
                .SetBasePath(baseDirectory)
                .AddJsonFile("yams.config.json", false)
                .AddJsonFile("yams.secrets.json", true)
                .AddJsonFile($"yams.{environment}.config.json", true)
                .AddJsonFile($"yams.{environment}.secrets.json", true);
            var config = new YamsServiceConfiguration(environment);
            var configRoot = builder.Build();
            configRoot.Bind(config);
            return config;
        }
    }
}
