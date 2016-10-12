using System;
using Conreign.Core.Communication;
using Conreign.Core.Gameplay;
using MongoDB.Bson.Serialization;

namespace Conreign.Host.Storage
{
    // TODO: move to a separate storage specific library
    public static  class MongoDriverConfiguration
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

            BsonClassMap.RegisterClassMap<BusState>();
            BsonClassMap.RegisterClassMap<PlayerState>();
        }
    }
}
