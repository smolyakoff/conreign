using System;
using Microsoft.Extensions.Configuration;

namespace Conreign.LoadTest.Supervisor
{
    public class LoadTestSupervisorOptions
    {
        public LoadTestSupervisorOptions()
        {
            InstanceOptions = new LoadTestOptions
            {
                Timeout = TimeSpan.FromMinutes(30)
            };
        }

        public string Name { get; set; } = $"{DateTime.UtcNow:yy-MM-dd__hh-mm-ss}";
        public string OutputDirectory { get; set; } = Environment.CurrentDirectory;
        public string ConfigurationFileName { get; set; }
        public string ApplicationVersion { get; set; }
        public string BatchUrl { get; set; }
        public string BatchAccountName { get; set; }
        public string BatchAccountKey { get; set; }
        public string PoolId { get; set; }
        public bool UseAutoPool { get; set; } = true;
        public string MachineSize { get; set; } = "small";
        public int Instances { get; set; } = 4;
        public bool DownloadLogFiles { get; set; } = true;
        public int FileDownloadDegreeOfParallelism { get; set; } = 2;
        public TimeSpan TaskStatisticsFetchInterval { get; set; } = TimeSpan.FromSeconds(10);
        public TimeSpan JobTimeout { get; set; } = TimeSpan.FromHours(1);
        public LoadTestOptions InstanceOptions { get; set; }

        public static LoadTestSupervisorOptions Parse(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            var configFileName = config.GetValue<string>(nameof(ConfigurationFileName), null);
            if (configFileName != null)
            {
                builder = new ConfigurationBuilder();
                builder.AddJsonFile(configFileName, false);
                builder.AddCommandLine(args);
            }
            config = builder.Build();
            var options = new LoadTestSupervisorOptions();
            config.Bind(options);
            return options;
        }
    }
}