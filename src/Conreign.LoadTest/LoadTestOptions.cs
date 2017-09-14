using System;
using System.Diagnostics;
using System.Net;
using Conreign.LoadTest.Core;
using Microsoft.Extensions.Configuration;
using Serilog.Events;

namespace Conreign.LoadTest
{
    public class LoadTestOptions
    {
        private const string ConfigurationFileKey = "ConfigurationFileName";

        public LoadTestOptions()
        {
            ConnectionUri = "http://localhost:3000";
            MinimumLogLevel = LogEventLevel.Information;
            var process = Process.GetCurrentProcess();
            InstanceId = $"{Dns.GetHostName()}({process.Id})";
            Timeout = TimeSpan.FromHours(1);
            BotOptions = new LoadTestBotOptions
            {
                RoomPrefix = $"{InstanceId}:"
            };
        }

        public string InstanceId { get; set; }
        public string ConfigurationFileName { get; set; }
        public string LogFileName { get; set; } = "log.json";
        public bool LogToConsole { get; set; } = true;
        public LogEventLevel MinimumConsoleLogLevel { get; set; } = LogEventLevel.Information;
        public bool ZipLogFile { get; set; } = true;
        public string ConnectionUri { get; set; }
        public string ElasticSearchUri { get; set; }
        public TimeSpan ElasticSearchFlushInterval { get; set; } = TimeSpan.FromSeconds(5);
        public LogEventLevel MinimumLogLevel { get; set; }
        public LoadTestBotOptions BotOptions { get; set; }
        public TimeSpan Timeout { get; set; }
        public TimeSpan GracefulStopPeriod { get; set; } = TimeSpan.FromMinutes(10);
        public TimeSpan LogFlushPeriod { get; set; } = TimeSpan.FromMinutes(5);

        public static LoadTestOptions Parse(string[] args)
        {
            var builder = new ConfigurationBuilder();
            builder.AddCommandLine(args);
            var config = builder.Build();

            var configFileName = config.GetValue<string>(ConfigurationFileKey, null);
            if (configFileName != null)
            {
                builder = new ConfigurationBuilder();
                builder.AddJsonFile(configFileName, false);
                builder.AddCommandLine(args);
            }

            config = builder.Build();
            var options = new LoadTestOptions();
            config.Bind(options);
            return options;
        }
    }
}