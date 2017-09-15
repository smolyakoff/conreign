using System.Linq;
using Conreign.Contracts.Communication;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;
using Orleans.MongoStorageProvider.Driver;

namespace Conreign.Server.Silo
{
    internal class MongoDriverBootstrap : IMongoDriverBootstrap
    {
        public void Init()
        {
            var contractsAssembly = typeof(IClientEvent).Assembly;
            var eventTypes = contractsAssembly
                .GetExportedTypes()
                .Where(t => t.IsClass && typeof(IClientEvent).IsAssignableFrom(t))
                .ToList();
            foreach (var type in eventTypes)
                BsonClassMap.LookupClassMap(type);
            var conventionPack = new ConventionPack
            {
                new DictionaryRepresentationConvention(DictionaryRepresentation.ArrayOfArrays)
            };
            ConventionRegistry.Register(
                "Conreign",
                conventionPack,
                t => true);
        }
    }
}