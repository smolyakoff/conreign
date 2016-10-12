using System;
using MongoDB.Bson.Serialization.Attributes;

namespace Microsoft.Orleans.Storage
{
    internal class MongoGrainMeta
    {
        public MongoGrainMeta()
        {
            Timestamp = DateTime.UtcNow.Ticks;
        }

        [BsonRequired]
        [BsonElement("ts")]
        public long Timestamp { get; set; }

        [BsonIgnore]
        public string ETag
        {
            get { return FormatETag(Timestamp); }
            set { Timestamp = ParseETag(value); }
        }

        [BsonRequired]
        [BsonElement("grain_id")]
        public string GrainId { get; set; }

        [BsonRequired]
        [BsonElement("grain_type")]
        public string GrainType { get; set; }

        [BsonRequired]
        [BsonElement("grain_state_type")]
        public string GrainStateType { get; set; }

        [BsonRequired]
        [BsonElement("service_id")]
        public string ServiceId { get; set; }

        public override string ToString()
        {
            return
                $"GrainId={GrainId}, GrainType={GrainType}, GrainStateType={GrainStateType}, ServiceId={ServiceId}, ETag={ETag ?? "<null>"}";
        }

        private static string FormatETag(long value)
        {
            return value.ToString();
        }

        private static long ParseETag(string etag)
        {
            if (string.IsNullOrEmpty(etag))
            {
                return 0;
            }
            long ticks;
            var valid = long.TryParse(etag, out ticks);
            if (!valid)
            {
                throw new ArgumentException("Invalid ETag format. Should be a string with DateTime.Ticks value.");
            }
            return ticks;
        }
    }
}