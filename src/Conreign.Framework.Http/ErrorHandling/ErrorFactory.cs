using System;
using System.Linq;
using System.Net;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Framework.Http.ErrorHandling
{
    public class ErrorFactory
    {
        private readonly ErrorFactorySettings _settings;

        public ErrorFactory(ErrorFactorySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            _settings = settings;
        }

        public HttpError Create(Exception ex)
        {
            var error = new Failure(ex);
            return Create(error);
        }

        public HttpError Create(IError error)
        {
            dynamic dynamicError = error;
            HttpError httpError = CreateBase(dynamicError);
            var mappers = _settings.StatusCodeMappers ?? Enumerable.Empty<IHttpErrorStatusCodeMapper>();
            var statusCode = mappers.Select(m => m.GetStatusCodeForError(error)).FirstOrDefault(x => x != null);
            httpError.Type = error.Type;
            httpError.StatusCode = statusCode ??
                                   (error is UserError ? HttpStatusCode.BadRequest : HttpStatusCode.InternalServerError);
            return httpError;
        }

        private HttpError CreateBase(UserError userError)
        {
            var message = userError.Message;
            return new HttpError {Message = message, Info = userError.Info};
        }

        private HttpError CreateBase(Failure failure)
        {
            var message = _settings.SerializeExceptionErrorMessage
                ? failure.Exception.Message
                : "Unexpected system error occurred.";
            var stackTrace = _settings.SerializeStackTrace ? failure.Exception.ToString() : null;
            return new HttpError {Message = message, StackTrace = stackTrace};
        }
    }
}