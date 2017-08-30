using System.Linq;
using Conreign.Core.Contracts.Communication;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Conventions;
using MongoDB.Bson.Serialization.Options;

namespace Conreign.Cluster
{
    public static class MongoDriverConfiguration
    {
        private static bool _isInitialized;

        public static void EnsureInitialized()
        {
            if (_isInitialized)
            {
                return;
            }
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
                t => t.Namespace != null && t.Namespace.StartsWith("Conreign"));
            _isInitialized = true;          
        }
    }
}