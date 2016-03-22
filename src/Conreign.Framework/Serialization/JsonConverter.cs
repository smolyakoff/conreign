using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Conreign.Framework.Core.Serialization
{
    public class JsonConverter : IConverter
    {
        private readonly JsonSerializer _serializer;

        public JsonConverter() : this(JsonConvert.DefaultSettings())
        {
        }

        public JsonConverter(JsonSerializerSettings settings)
        {
            _serializer = JsonSerializer.Create(settings);
        }

        public T Convert<T>(object source)
        {
            if (source == null)
            {
                return default(T);
            }
            var jObject = JObject.FromObject(source, _serializer);
            return jObject.ToObject<T>();
        }

        public object Convert(object source, Type targetType)
        {
            if (source == null)
            {
                return null;
            }
            var jObject = JObject.FromObject(source, _serializer);
            return jObject.ToObject(targetType);
        }
    }
}