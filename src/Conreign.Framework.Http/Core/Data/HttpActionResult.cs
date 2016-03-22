using System.Net;

namespace Conreign.Framework.Http.Core.Data
{
    public class HttpActionResult
    {
        public HttpActionResult(HttpStatusCode statusCode, object payload)
        {
            StatusCode = statusCode;
            Payload = payload;
        }

        public HttpStatusCode StatusCode { get; }

        public object Payload { get; }
    }
}