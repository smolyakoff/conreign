using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Conreign.Server.Contracts.Client.Serialization;

public class MessageEnvelopeJsonConverter : JsonConverter
{
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        if (value == null)
        {
            writer.WriteNull();
            return;
        }

        var dto = ToDto((dynamic)value);
        serializer.Serialize(writer, dto);
    }

    public override bool CanConvert(Type objectType)
    {
        return typeof(MessageEnvelope) == objectType;
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
        JsonSerializer serializer)
    {
        if (reader.TokenType == JsonToken.Null)
        {
            return null;
        }

        if (reader.TokenType != JsonToken.StartObject)
        {
            throw new NotSupportedException($"Cannot deserialize command envelope from {reader.TokenType}.");
        }

        dynamic envelope = existingValue ?? Activator.CreateInstance(objectType);
        var jObject = serializer.Deserialize<JObject>(reader);
        var typeToken = jObject.GetValue("type", StringComparison.OrdinalIgnoreCase);
        if (typeToken == null || typeToken.Type != JTokenType.String)
        {
            throw new InvalidOperationException(
                $"Expected to get type string field from json object, but got {typeToken}.");
        }

        var payloadType = MessageTypeSerializer.Deserialize(typeToken.Value<string>());
        var payloadToken = jObject.GetValue("payload", StringComparison.OrdinalIgnoreCase);
        dynamic payload = payloadToken == null
            ? Activator.CreateInstance(payloadType)
            : serializer.Deserialize(payloadToken.CreateReader(), payloadType);
        var metaToken = jObject.GetValue("meta", StringComparison.OrdinalIgnoreCase);
        var meta = metaToken == null ? new Metadata() : serializer.Deserialize<Metadata>(metaToken.CreateReader());
        envelope.Payload = payload;
        envelope.Meta = meta;
        return envelope;
    }


    private static object ToDto(MessageEnvelope envelope)
    {
        return new
        {
            envelope.Type,
            envelope.Payload,
            envelope.Meta
        };
    }
}