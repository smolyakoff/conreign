using System.Net;
using Conreign.Api.Framework.ErrorHandling;

namespace Conreign.Api.Framework
{
    public class HttpActionResult
    {
        public static HttpActionResult FromError(IError error)
        {
            return new HttpActionResult(error.StatusCode, error);
        }

        public HttpActionResult(HttpStatusCode statusCode, object payload)
        {
            StatusCode = statusCode;
            Payload = payload;
        }

        public HttpStatusCode StatusCode { get; }

        public object Payload { get; }
    }
}
