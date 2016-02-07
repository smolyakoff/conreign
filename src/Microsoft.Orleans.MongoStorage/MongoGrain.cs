using System.Collections.Generic;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson.Serialization.Options;

namespace Microsoft.Orleans.Storage
{
    internal class MongoGrain
    {
        [BsonId]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("meta")]
        public MongoGrainMeta Meta { get; set; }

        [BsonDictionaryOptions(DictionaryRepresentation.Document)]
        [BsonElement("data")]
        public Dictionary<string, object> Data { get; set; }
    }
}