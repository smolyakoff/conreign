using MediatR;
using Newtonsoft.Json.Linq;

namespace Conreign.Api.Framework
{
    public class HttpAction : IAsyncRequest<HttpActionResult>
    {
        public string Type { get; set; }

        public JObject Meta { get; set; }

        public JObject Payload { get; set; }
    }
}
