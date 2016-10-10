using System;
using MongoDB.Bson.Serialization;
using Orleans.Runtime;

namespace Microsoft.Orleans.Storage.Serialization
{
    public class OrleansSerializerProvider : IBsonSerializationProvider
    {
        public IBsonSerializer GetSerializer(Type type)
        {
            if (typeof(Type) == type)
            {
                return new TypeSerializer();
            }
            if (typeof(GrainReference).IsAssignableFrom(type))
            {
                return new GrainReferenceSerializer();
            }
            return null;
        }
    }
}
