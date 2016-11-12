using System;
using System.Diagnostics;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Conreign.Client.SignalR;
using Conreign.Core.AI;
using Conreign.Core.AI.LoadTest;
using Serilog;
using Serilog.Formatting.Json;
using Serilog.Sinks.Elasticsearch;

namespace Conreign.LoadTest
{
    public static class LoadTestRunner
    {
        public static Task Run(string[] args)
        {
            var options = LoadTestOptions.Parse(args);
            return Run(options);
        }

        public static async Task Run(LoadTestOptions options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            var loggerConfiguration = new LoggerConfiguration();
            if (options.LogToConsole)
            {
                loggerConfiguration.WriteTo.LiterateConsole();
            }
            if (!string.IsNullOrEmpty(options.LogFileName))
            {
                var formatter = new JsonFormatter(renderMessage: true, closingDelimiter: ",");
                var logFilePath = Environment.ExpandEnvironmentVariables(options.LogFileName);
                loggerConfiguration.WriteTo.File(formatter, logFilePath, buffered: true);
            }
            if (!string.IsNullOrEmpty(options.ElasticSearchUri))
            {
                var elasticOptions = new ElasticsearchSinkOptions(new Uri(options.ElasticSearchUri))
                {
                    AutoRegisterTemplate = true,
                    BufferBaseFilename = "logs/elastic-buffer"
                };
                loggerConfiguration.WriteTo.Elasticsearch(elasticOptions);
            }
            Log.Logger = loggerConfiguration
                .MinimumLevel.Is(options.MinimumLogLevel)
                .Enrich.FromLogContext()
                .CreateLogger()
                .ForContext("ApplicationId", "Conreign.LoadTest")
                .ForContext("InstanceId", options.InstanceId);
            Serilog.Debugging.SelfLog.Enable(msg => Trace.WriteLine(msg));
            var logger = Log.Logger;
            try
            {
                var botOptions = options.BotOptions;
                logger.Information("Initialized with the following configuration: {@Configuration}", options);
                ServicePointManager.DefaultConnectionLimit = botOptions.RoomsCount * botOptions.BotsPerRoomCount * 2;
                var signalrOptions = new SignalRClientOptions(options.ConnectionUri);
                var client = new SignalRClient(signalrOptions);
                var factory = new LoadTestBotFactory(botOptions);
                var farm = new BotFarm(options.InstanceId, client, factory, new BotFarmOptions());
                var cts = new CancellationTokenSource();
                cts.CancelAfter(options.Timeout);

                await farm.Run(cts.Token);
            }
            catch (Exception ex)
            {
                logger.Fatal(ex, "Load test failed: {ErrorMessage}.", ex.Message);
                throw;
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }
    }
}
