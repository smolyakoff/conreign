using System;

namespace Conreign.Framework.Contracts.Core.Data
{
    public class UserError<T> : UserError, IError<T>
    {
        public UserError(UserException<T> exception, string title = null) : this(exception.Message, exception, title)
        {
        }

        public UserError(string message, UserException<T> exception, string title = null) : this(message, exception.Type, exception, title)
        {
        }

        public UserError(string message, T type, Exception exception = null, string title = null)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            Message = message;
            Type = type;
            Title = title;
            Exception = exception;
        }

        public string Title { get; }

        public string Message { get; }

        public T Type { get; }

        public Exception Exception { get; }

        public override string ToString()
        {
            return $"USER ERROR [{Type}] - {Message}";
        }
    }
}