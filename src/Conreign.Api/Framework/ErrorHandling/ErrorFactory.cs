using System;
using System.Net;
using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Api.Framework.ErrorHandling
{
    public class ErrorFactory
    {
        private readonly ErrorFactorySettings _settings;

        public ErrorFactory() : this(new ErrorFactorySettings())
        {
        }

        public ErrorFactory(ErrorFactorySettings settings)
        {
            if (settings == null)
            {
                throw new ArgumentNullException(nameof(settings));
            }
            _settings = settings;
        }

        public IError Create(Exception exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            return SerializeInternal((dynamic)exception);
        }

        private static UserError SerializeInternal(UserException exception)
        {
            return new UserError(exception);
        }

        private SystemError SerializeInternal(Exception exception)
        {
            return CreateSystemError(exception);
        }

        private SystemError CreateSystemError(Exception exception, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            var message = _settings.SerializeSystemErrorMessage
                ? exception.Message
                : "Unexpected system error occurred.";
            var error = new SystemError(exception, message, statusCode, _settings.SerializeStackTrace);
            return error;
        }
    }
}
