using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Conreign.Core.Contracts.Abstractions;
using Conreign.Core.Contracts.Abstractions.Exceptions;
using Conreign.Core.Contracts.Auth;

namespace Conreign.Api.Framework.ErrorHandling
{
    public class ErrorFactory
    {
        private readonly ErrorFactorySettings _settings;

        private static readonly Dictionary<string, HttpStatusCode> Codes;

        static ErrorFactory()
        {
            Codes = new Dictionary<string, HttpStatusCode>
            {
                [AuthErrors.TokenExpired.Type] = HttpStatusCode.Unauthorized
            };
        }

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

        private IError SerializeInternal(AggregateException exception)
        {
            return SerializeInternal((dynamic)exception.InnerException);
        }

        private static UserError SerializeInternal(UserException exception)
        {
            var code = Codes.ContainsKey(exception.Type) ? Codes[exception.Type] : HttpStatusCode.BadRequest;
            return new UserError(exception, code);
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
