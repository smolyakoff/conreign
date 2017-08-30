using System;
using MongoDB.Bson.Serialization;
using Orleans.Runtime;

namespace Microsoft.Orleans.MongoStorage.Driver
{
    public class OrleansSerializerProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (typeof(GrainReference).IsAssignableFrom(type))
            {
                return new GrainReferenceSerializer();
            }
            return null;
        }
    }
}