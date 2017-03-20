using System;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Gameplay.Events;
using Conreign.Core.Gameplay;
using Conreign.Core.Presence;
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

            BsonClassMap.RegisterClassMap<MapData>();
            BsonClassMap.RegisterClassMap<ConnectionState>();
            BsonClassMap.RegisterClassMap<PlayerState>();
            BsonClassMap.RegisterClassMap<LobbyState>();
            BsonClassMap.RegisterClassMap<GameState>();
        }
    }
}