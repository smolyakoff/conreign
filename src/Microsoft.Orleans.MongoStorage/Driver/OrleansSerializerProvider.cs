using System;
using MongoDB.Bson.Serialization;
using Orleans.Runtime;

namespace Microsoft.Orleans.MongoStorage.Driver
{
    public class OrleansSerializerProvider : IBsonSerializationProvider
    {
        private readonly IGrainReferenceConverter _grainReferenceConverter;

        public OrleansSerializerProvider(IGrainReferenceConverter grainReferenceConverter)
        {
            _grainReferenceConverter = grainReferenceConverter ?? throw new ArgumentNullException(nameof(grainReferenceConverter));
        }

        public IBsonSerializer GetSerializer(Type type)
        {
            if (typeof(GrainReference).IsAssignableFrom(type))
            {
                return new GrainReferenceSerializer(_grainReferenceConverter);
            }
            return null;
        }
    }
}