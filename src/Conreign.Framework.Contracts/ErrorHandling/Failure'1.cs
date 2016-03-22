using System;
using Conreign.Framework.Contracts.Core.Data;

namespace Conreign.Framework.Contracts.ErrorHandling
{
    public class Failure<T> : IError<T>
    {
        public Failure(Exception exception, T type) : this(exception, exception.Message, type)
        {
        }

        public Failure(Exception exception, string message, T type)
        {
            Exception = exception;
            Message = message;
            Type = type;
        }

        public Failure(string message, T type)
        {
            Message = message;
            Type = type;
        }

        public string Message { get; }

        public Exception Exception { get; }

        public T Type { get; }
    }
}
