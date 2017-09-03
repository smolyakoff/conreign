using System.Linq;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using Orleans.Providers;

namespace Conreign.Cluster
{
    internal class MongoDriverBootstrapProvider : IBootstrapProvider
    {
        public Task Init(string name, IProviderRuntime providerRuntime, IProviderConfiguration config)
        {
            var contractsAssembly = typeof(IClientEvent).Assembly;
            var eventTypes = contractsAssembly
                .GetExportedTypes()
                .Where(t => t.IsClass && typeof(IClientEvent).IsAssignableFrom(t))
                .ToList();
            foreach (var type in eventTypes)
            {
                BsonClassMap.LookupClassMap(type);
            }
            var conventionPack = new ConventionPack
            {
                new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays),
            };
            ConventionRegistry.Register(
                "Conreign",
                conventionPack,
                t => true);
            return Task.FromResult(0);
        }

        public Task Close()
        {
            return Task.FromResult(0);
        }

        public string Name { get; } = "MongoDriverBootstrapProvider";
    }
}