using System;
using System.Diagnostics;
using System.Net;
using Conreign.Core.AI.LoadTest;
using Serilog.Events;

namespace Conreign.LoadTest
{
    public class LoadTestOptions
    {
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
        public string ConnectionUri { get; set; }
        public LogEventLevel MinimumLogLevel { get; set; }
        public LoadTestBotOptions BotOptions { get; set; }
        public TimeSpan Timeout { get; set; }
    }
}