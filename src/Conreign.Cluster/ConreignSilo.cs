using System;
using Conreign.Cluster.Storage;
using Conreign.Core.Contracts.Communication;
using Microsoft.Orleans.Storage;
using Orleans.Runtime.Configuration;

namespace Conreign.Cluster
{
    public class ConreignSilo
    {
        public ClusterConfiguration OrleansConfiguration { get; }
        public ConreignSiloConfiguration Configuration { get; }

        public static ConreignSilo Configure(ClusterConfiguration baseOrleansConfiguration = null, ConreignSiloConfiguration conreignConfiguration = null)
        {
            conreignConfiguration = conreignConfiguration ?? ConreignSiloConfiguration.Load(Environment.CurrentDirectory);
            var orleansConfig = baseOrleansConfiguration ?? ClusterConfiguration.LocalhostPrimarySilo();
            orleansConfig.Globals.LivenessType = conreignConfiguration.LivenessStorageType;
            orleansConfig.Globals.DataConnectionString = conreignConfiguration.SystemStorageConnectionString;
            orleansConfig.Globals.ReminderServiceType = conreignConfiguration.RemindersStorageType;
            orleansConfig.Globals.DataConnectionStringForReminders = conreignConfiguration.SystemStorageConnectionString;
            orleansConfig.AddSimpleMessageStreamProvider(StreamConstants.ProviderName);
            orleansConfig.AddMemoryStorageProvider("PubSubStore");
            switch (conreignConfiguration.DataStorageType)
            {
                case StorageType.AzureTable:
                    orleansConfig.AddAzureTableStorageProvider("Default",
                        conreignConfiguration.DataStorageConnectionString);
                    break;
                case StorageType.MongoDb:
                    orleansConfig.AddMongoDbStorageProvider("Default", 
                        conreignConfiguration.DataStorageConnectionString);
                    break;
                case StorageType.InMemory:
                    orleansConfig.AddMemoryStorageProvider("Default");
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            return new ConreignSilo(orleansConfig, conreignConfiguration);
        }

        private ConreignSilo(ClusterConfiguration orleansConfiguration, ConreignSiloConfiguration configuration)
        {
            OrleansConfiguration = orleansConfiguration;
            Configuration = configuration;
        }

        public void Initialize()
        {
            MongoDriverConfiguration.EnsureInitialized();
        }
    }
}
