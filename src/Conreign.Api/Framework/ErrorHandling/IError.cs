using System.Net;

namespace Conreign.Api.Framework.ErrorHandling
{
    public interface IError
    {
        string Message { get; }

        string Type { get; }

        HttpStatusCode StatusCode { get; }
    }
}
