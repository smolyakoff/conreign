using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Conreign.Server.Auth.Serialization
{
    public class SecondsSinceEpochConverter : DateTimeConverterBase
    {
        private static readonly DateTime Epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var seconds = Convert.ToInt32(((DateTime) value - Epoch).TotalSeconds).ToString();
            writer.WriteRawValue(seconds);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue,
            JsonSerializer serializer)
        {
            if (reader.Value == null)
            {
                return null;
            }
            return Epoch.AddSeconds((long) reader.Value);
        }
    }
}