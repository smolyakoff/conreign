using System;
using System.Collections.Generic;
using System.Reflection;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Gameplay;
using Orleans.MongoStorageProvider.Configuration;
using Orleans.Runtime;
using Orleans.Runtime.Configuration;
using Orleans.Telemetry.SerilogConsumer;
using Serilog;
using Serilog.Core.Enrichers;

namespace Conreign.Server.Silo
{
    public class ConreignSilo
    {
        private ConreignSilo(ConreignSiloConfiguration configuration, ILogger logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public ConreignSiloConfiguration Configuration { get; }
        public ILogger Logger { get; }

        public ClusterConfiguration CreateOrleansConfiguration()
        {
            var config = Configuration.LivenessStorageType == GlobalConfiguration.LivenessProviderType.MembershipTableGrain
                ? ClusterConfiguration.LocalhostPrimarySilo()
                : new ClusterConfiguration();
            config.Globals.DeploymentId = Configuration.ClusterId;
            config.Globals.LivenessType = Configuration.LivenessStorageType;
            config.Globals.DataConnectionString = Configuration.SystemStorageConnectionString;
            config.Globals.ReminderServiceType = Configuration.RemindersStorageType;
            config.Globals.DataConnectionStringForReminders = Configuration.SystemStorageConnectionString;
            config.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);
            config.AddMemoryStorageProvider("PubSubStore");
            switch (Configuration.DataStorageType)
            {
                case StorageType.AzureTable:
                    config.AddAzureTableStorageProvider("Default", Configuration.DataStorageConnectionString);
                    break;
                case StorageType.MongoDb:
                    var grainAssemblies = new List<Assembly> { typeof(GameGrain).Assembly };
                    var bootstrapAssemblies = new List<Assembly> { Assembly.GetExecutingAssembly() };
                    var options = new MongoStorageOptions(
                        Configuration.DataStorageConnectionString,
                        grainAssemblies,
                        bootstrapAssemblies);
                    config.AddMongoDbStorageProvider("Default", options);
                    break;
                case StorageType.InMemory:
                    config.AddMemoryStorageProvider("Default");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            config.UseStartupType<SiloStartup>();
            return config;
        }

        public static ConreignSilo Create(ConreignSiloConfiguration configuration)
        {
            if (configuration == null)
            {
                throw new ArgumentNullException(nameof(configuration));
            }
            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(configuration.MinimumLogLevel)
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .CreateLogger()
                .ForContext(new []
                {
                    new PropertyEnricher("ApplicationId", "Conreign.Silo"), 
                    new PropertyEnricher("ClusterId", configuration.ClusterId),
                    new PropertyEnricher("InstanceId", configuration.InstanceId) 
                });
            // HACK: Side-effects here but what can I do with static classes :(
            var consumer = new SerilogConsumer(logger);
            SiloStartup.Configuration = configuration;
            LogManager.LogConsumers.Add(consumer);
            LogManager.TelemetryConsumers.Add(consumer);
            Log.Logger = logger;
            return new ConreignSilo(configuration, logger);
        }
    }
}