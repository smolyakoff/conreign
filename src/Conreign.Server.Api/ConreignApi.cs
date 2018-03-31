using System;
using System.IO;
using Conreign.Server.Api.Configuration;
using Conreign.Server.Contracts.Communication;
using Orleans.Runtime.Configuration;
using Serilog;
using Serilog.Core.Enrichers;
using Serilog.Sinks.Elasticsearch;

namespace Conreign.Server.Api
{
    public class ConreignApi
    {
        private ConreignApi(ConreignApiConfiguration apiConfiguration, ILogger logger)
        {
            Configuration = apiConfiguration;
            Logger = logger;
        }

        public ConreignApiConfiguration Configuration { get; }
        public ILogger Logger { get; }

        public ClientConfiguration CreateOrleansConfiguration()
        {
            var config = Configuration.SystemStorageType == ClientConfiguration.GatewayProviderType.Config
                ? ClientConfiguration.LocalhostSilo()
                : new ClientConfiguration();
            config.DeploymentId = Configuration.ClusterId;
            config.GatewayProvider = Configuration.SystemStorageType;
            config.DataConnectionString = Configuration.SystemStorageConnectionString;
            config.AddSimpleMessageStreamProvider(
                StreamConstants.ProviderName, 
                fireAndForgetDelivery: true,
                optimizeForImmutableData: false);
            return config;
        }

        public static ConreignApi Create(ConreignApiConfiguration apiConfiguration)
        {
            if (apiConfiguration == null)
            {
                throw new ArgumentNullException(nameof(apiConfiguration));
            }
            var loggerConfiguration = new LoggerConfiguration();
            if (!string.IsNullOrEmpty(apiConfiguration.ElasticSearchUri))
            {
                var elasticOptions = new ElasticsearchSinkOptions(new Uri(apiConfiguration.ElasticSearchUri))
                {
                    AutoRegisterTemplate = true,
                    BufferBaseFilename = "logs/elastic-buffer"
                };
                loggerConfiguration.WriteTo.Elasticsearch(elasticOptions);
            }
            var logger = loggerConfiguration
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile(Path.Combine(ApplicationPath.CurrentDirectory, "logs", "conreign-api-{Date}.log"))
                .MinimumLevel.Is(apiConfiguration.MinimumLogLevel)
                .Enrich.FromLogContext()
                .CreateLogger()
                .ForContext(new []
                {
                    new PropertyEnricher("ApplicationId", "Conreign.Api"),
                    new PropertyEnricher("ClusterId", apiConfiguration.ClusterId),
                    new PropertyEnricher("InstanceId", apiConfiguration.InstanceId) 
                });
            return new ConreignApi(apiConfiguration, logger);
        }
    }
}