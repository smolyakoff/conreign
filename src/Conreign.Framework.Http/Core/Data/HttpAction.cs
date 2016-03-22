using MediatR;
using Newtonsoft.Json.Linq;

namespace Conreign.Framework.Http.Core.Data
{
    public class HttpAction : IAsyncRequest<HttpActionResult>
    {
        public string Type { get; set; }

        public JObject Meta { get; set; }

        public JObject Payload { get; set; }
    }
}