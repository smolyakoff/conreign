using MediatR;
using Newtonsoft.Json.Linq;

namespace Conreign.Api.Framework
{
    public class GenericAction : IAsyncRequest<GenericActionResult>
    {
        public string Type { get; set; }

        public JObject Meta { get; set; }

        public JObject Payload { get; set; }
    }
}
