using System;
using System.Linq.Expressions;
using System.Net;
using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Api.Framework.ErrorHandling
{
    public class SystemError : IError
    {
        public SystemError(Exception exception, bool copyStackTrace = true) : this(exception, null, null, HttpStatusCode.InternalServerError, copyStackTrace)
        {
        }

        public SystemError(Exception exception, string message, bool copyStackTrace = true) : this(exception, message, null, HttpStatusCode.InternalServerError, copyStackTrace)
        {
        }

        public SystemError(Exception exception, string message, HttpStatusCode statusCode, bool copyStackTrace = true) : this(exception, message, null, statusCode, copyStackTrace)
        {
        }

        public SystemError(Exception exception, HttpStatusCode statusCode, bool copyStackTrace = true) : this(exception, null, null, statusCode, copyStackTrace)
        {
        }

        public SystemError(string message) : this(message, null)
        {
        }

        public SystemError(string message, HttpStatusCode statusCode) : this(message, null, statusCode)
        {
        }

        public SystemError(TypedMessage message, HttpStatusCode statusCode = HttpStatusCode.InternalServerError) : this(message?.Text, message?.Type, statusCode)
        {
        }

        public SystemError(string message, string type, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message should not be empty.", nameof(message));
            }
            if (string.IsNullOrEmpty(type))
            {
                throw new ArgumentException("Type should not be empty.", nameof(type));
            }
            Message = message;
            Type = type;
            StatusCode = statusCode;
        }

        public SystemError(Exception exception, 
            string message, 
            string type, 
            HttpStatusCode statusCode = HttpStatusCode.InternalServerError,
            bool copyStackTrace = true)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            if (copyStackTrace)
            {
                StackTrace = exception.ToString();
            }
            Message = string.IsNullOrEmpty(message) ? exception.Message : message;
            Type = string.IsNullOrEmpty(type) ? exception.GetType().Name.Replace("Exception", string.Empty) : type;
            StatusCode = statusCode;
        }

        public static SystemError ForResource(Expression<Func<string>> messageGetter)
        {
            return new SystemError(TypedMessage.Create(messageGetter));
        }

        public static SystemError ServiceUnavailable()
        {
            return ForResource(() => Errors.ServiceUnavailable);
        }

        public string Message { get; }

        public string Type { get; }

        public HttpStatusCode StatusCode { get; }

        public string StackTrace { get; }
    }
}
