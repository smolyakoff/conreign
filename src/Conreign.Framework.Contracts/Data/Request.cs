using System.Collections.Generic;
using MediatR;

namespace Conreign.Framework.Contracts.Data
{
    public class Request : IAsyncRequest<Response>
    {
        public string Type { get; set; }

        public Dictionary<string, object> Payload { get; set; }

        public Dictionary<string, object> Meta { get; set; }
    }
}