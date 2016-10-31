using System;
using System.Linq;
using Conreign.Core.Contracts.Client.Exceptions;

namespace Conreign.Core.Contracts.Client.Messages
{
    public static class UserErrorExtensions
    {
        public static UserException ToUserException(this UserError error)
        {
            if (error == null)
            {
                throw new ArgumentNullException(nameof(error));
            }
            var type = error.GetType();
            if (!type.IsGenericType)
            {
                throw new NotSupportedException($"Expected to get a generic UserError. Got: {type.Name}.");
            }
            dynamic dynamicError = error;
            if (type.GetGenericTypeDefinition() == typeof(UserError<>))
            {
                var errorType = type.GetGenericArguments().First();
                var userErrorType = typeof(UserException<>).MakeGenericType(errorType);
                return (UserException)Activator.CreateInstance(userErrorType, dynamicError.Type, error.Message);
            }
            if (type.GetGenericTypeDefinition() == typeof(UserError<,>))
            {
                var genericArgs = type.GetGenericArguments();
                var userErrorType = typeof(UserException<,>).MakeGenericType(genericArgs[0], genericArgs[1]);
                return (UserException)Activator.CreateInstance(userErrorType, dynamicError.Type, dynamicError.Details, error.Message);
            }
            throw new NotSupportedException($"Error of type {type.Name} is not supported.");
        }

        public static UserException<T> ToUserException<T>(this UserError<T> error) where T : struct
        {
            return UserException.Create(error.Type, error.Message);
        }

        public static UserException<T, TDetails> ToUserException<T, TDetails>(this UserError<T, TDetails> error) where T : struct
        {
            return UserException.Create(error.Type, error.Details, error.Message);
        }
    }
}
