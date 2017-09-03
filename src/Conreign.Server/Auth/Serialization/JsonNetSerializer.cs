using JWT;
using Newtonsoft.Json;

namespace Conreign.Server.Auth.Serialization
{
    internal class JsonNetSerializer : IJsonSerializer
    {
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }

        public T Deserialize<T>(string json)
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
    }
}