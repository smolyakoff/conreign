using System.Net;

namespace Conreign.Framework.Http.ErrorHandling
{
    public class HttpError
    {
        public string Message { get; internal set; }

        public string Type { get; internal set; }

        public object Info { get; internal set; }

        public string StackTrace { get; internal set; }

        public HttpStatusCode StatusCode { get; internal set; }
    }
}