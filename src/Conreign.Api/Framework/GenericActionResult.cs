using System.Net;
using Conreign.Api.Framework.ErrorHandling;

namespace Conreign.Api.Framework
{
    public class GenericActionResult
    {
        public static GenericActionResult FromError(IError error)
        {
            return new GenericActionResult(error.StatusCode, error);
        }

        public GenericActionResult(HttpStatusCode statusCode, object payload)
        {
            StatusCode = statusCode;
            Payload = payload;
        }

        public HttpStatusCode StatusCode { get; }

        public object Payload { get; }
    }
}
