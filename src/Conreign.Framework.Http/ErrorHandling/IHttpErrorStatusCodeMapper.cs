using System.Net;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Framework.Http.ErrorHandling
{
    public interface IHttpErrorStatusCodeMapper
    {
        HttpStatusCode? GetStatusCodeForError(IError error);
    }
}