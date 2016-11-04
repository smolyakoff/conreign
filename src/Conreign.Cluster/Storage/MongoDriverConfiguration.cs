using System;
using Conreign.Core.Gameplay;
using Conreign.Core.Presence;
using Conreign.Host.Storage;
using MongoDB.Bson.Serialization;

namespace Conreign.Cluster.Storage
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
            _isInitialized = true;
            BsonSerializer.RegisterSerializer(typeof(Type), new TypeSerializer());
            BsonSerializer.RegisterSerializer(Type.GetType("System.RuntimeType"), new TypeSerializer());

            BsonClassMap.RegisterClassMap<ConnectionState>();
            BsonClassMap.RegisterClassMap<PlayerState>();
        }
    }
}