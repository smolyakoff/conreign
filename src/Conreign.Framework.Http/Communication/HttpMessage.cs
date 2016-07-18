using Newtonsoft.Json.Linq;

namespace Conreign.Framework.Http.Communication
{
    public class HttpMessage
    {
        public string Type { get; set; }

        public JObject Payload { get; set; }

        public JObject Meta { get; set; }
    }
}
