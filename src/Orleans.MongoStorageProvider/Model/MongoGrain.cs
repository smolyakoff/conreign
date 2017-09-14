using MongoDB.Bson.Serialization.Attributes;

namespace Orleans.MongoStorageProvider.Model
{
    internal class MongoGrain
    {
        [BsonId]
        public string Id { get; set; }

        [BsonRequired]
        [BsonElement("meta")]
        public MongoGrainMeta Meta { get; set; }

        [BsonElement("data")]
        public object Data { get; set; }
    }
}