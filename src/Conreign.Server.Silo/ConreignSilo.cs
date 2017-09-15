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

namespace Conreign.Server.Silo
{
    public class ConreignSilo
    {
        private ConreignSilo(
            ClusterConfiguration orleansConfiguration, 
            ConreignSiloConfiguration configuration,
            ILogger logger)
        {
            OrleansConfiguration = orleansConfiguration;
            Configuration = configuration;
            Logger = logger;
        }

        public ClusterConfiguration OrleansConfiguration { get; }
        public ConreignSiloConfiguration Configuration { get; }
        public ILogger Logger { get; }

        public static ConreignSilo Create(
            ClusterConfiguration baseOrleansConfiguration = null,
            ConreignSiloConfiguration conreignConfiguration = null)
        {
            conreignConfiguration = conreignConfiguration ?? 
                ConreignSiloConfiguration.Load(Environment.CurrentDirectory, ConreignSiloConfiguration.DefaultEnvironment);
            var orleansConfig = baseOrleansConfiguration ?? ClusterConfiguration.LocalhostPrimarySilo();
            orleansConfig.Globals.LivenessType = conreignConfiguration.LivenessStorageType;
            orleansConfig.Globals.DataConnectionString = conreignConfiguration.SystemStorageConnectionString;
            orleansConfig.Globals.ReminderServiceType = conreignConfiguration.RemindersStorageType;
            orleansConfig.Globals.DataConnectionStringForReminders =
                conreignConfiguration.SystemStorageConnectionString;
            orleansConfig.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);
            orleansConfig.AddMemoryStorageProvider("PubSubStore");
            switch (conreignConfiguration.DataStorageType)
            {
                case StorageType.AzureTable:
                    orleansConfig.AddAzureTableStorageProvider("Default",
                        conreignConfiguration.DataStorageConnectionString);
                    break;
                case StorageType.MongoDb:
                    var grainAssemblies = new List<Assembly> {typeof(GameGrain).Assembly};
                    var bootstrapAssemblies = new List<Assembly> {Assembly.GetExecutingAssembly()};
                    var options = new MongoStorageOptions(
                        conreignConfiguration.DataStorageConnectionString,
                        grainAssemblies,
                        bootstrapAssemblies);
                    orleansConfig.AddMongoDbStorageProvider("Default", options);
                    break;
                case StorageType.InMemory:
                    orleansConfig.AddMemoryStorageProvider("Default");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var logger = new LoggerConfiguration()
                .MinimumLevel.Is(conreignConfiguration.MinimumLogLevel)
                .Enrich.FromLogContext()
                .WriteTo.LiterateConsole()
                .CreateLogger()
                .ForContext("ApplicationId", "Conreign.Silo");
            // HACK: Side-effect here but what can I do with static classes :(
            var consumer = new SerilogConsumer(logger);
            LogManager.LogConsumers.Add(consumer);
            LogManager.TelemetryConsumers.Add(consumer);
            return new ConreignSilo(orleansConfig, conreignConfiguration, logger);
        }
    }
}