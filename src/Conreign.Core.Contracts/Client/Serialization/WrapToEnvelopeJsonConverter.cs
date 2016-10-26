using System;
using Newtonsoft.Json;

namespace Conreign.Core.Contracts.Client.Serialization
{
    public class WrapToEnvelopeJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            var envelope = new MessageEnvelope {Payload = value};
            serializer.Serialize(writer, envelope);
        }

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var envelope = serializer.Deserialize<MessageEnvelope>(reader);
            return envelope.Payload;
        }
    }
}
