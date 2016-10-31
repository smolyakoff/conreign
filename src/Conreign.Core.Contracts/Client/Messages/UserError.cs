using System;
using System.Linq;
using Conreign.Core.Contracts.Client.Exceptions;

namespace Conreign.Core.Contracts.Client.Messages
{
    public abstract class UserError
    {
        protected UserError(string message)
        {
            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }
            Message = message;
        }

        public static UserError Create(UserException exception)
        {
            if (exception == null)
            {
                throw new ArgumentNullException(nameof(exception));
            }
            var exceptionType = exception.GetType();
            if (!exceptionType.IsGenericType)
            {
                throw new NotSupportedException($"Expected to get a generic UserException. Got: {exception.GetType().Name}.");
            }
            dynamic dynamicException = exception;
            if (exceptionType.GetGenericTypeDefinition() == typeof(UserException<>))
            {
                var errorType = exceptionType.GetGenericArguments().First();
                var userErrorType = typeof(UserError<>).MakeGenericType(errorType);
                return (UserError)Activator.CreateInstance(userErrorType, exception.Message, dynamicException.Type);
            }
            if (exceptionType.GetGenericTypeDefinition() == typeof(UserException<,>))
            {
                var genericArgs = exceptionType.GetGenericArguments();
                var userErrorType = typeof(UserError<,>).MakeGenericType(genericArgs[0], genericArgs[1]);
                return (UserError)Activator.CreateInstance(userErrorType, exception.Message, dynamicException.Type, dynamicException.Details);
            }
            throw new NotSupportedException($"Exception of type {exception.GetType().Name} is not supported.");
        }

        public static UserError<T> Create<T>(UserException<T> exception) where T : struct
        {
            return new UserError<T>(exception.Message, exception.Type);   
        }

        public static UserError<T, TDetails> Create<T, TDetails>(UserException<T, TDetails> exception) where T : struct
        {
            return new UserError<T, TDetails>(exception.Message, exception.Type, exception.Details);
        }

        public string Message { get; }
    }
}
