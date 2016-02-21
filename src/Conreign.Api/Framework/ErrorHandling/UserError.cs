using System;
using System.Net;
using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Api.Framework.ErrorHandling
{
    public class UserError : IError
    {
        public UserError(UserException exception, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            Message = exception.Message;
            Type = exception.Type;
            Info = (exception as UserException<object>)?.Info;
            StatusCode = statusCode;
        }

        public UserError(UserMessage message, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            Message = message.Text;
            Type = message.Type;
            Title = message.Title;
            StatusCode = statusCode;
        }

        public UserError(UserMessage message, object info, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            Message = message.Text;
            Type = message.Type;
            Info = info;
            Title = message.Title;
            StatusCode = statusCode;
        }

        public string Message { get; }
        public string Title { get; }
        public string Type { get; }
        public object Info { get; }
        public HttpStatusCode StatusCode { get; }

        public static UserError NotFound()
        {
            return new UserError(UserMessage.FromResource(() => Errors.NotFound), HttpStatusCode.NotFound);
        }
    }
}
