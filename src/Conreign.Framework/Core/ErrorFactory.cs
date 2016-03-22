using System;
using Conreign.Framework.Contracts.Core;
using Conreign.Framework.Contracts.Core.Data;
using Conreign.Framework.Contracts.ErrorHandling;
using Conreign.Framework.Contracts.Routing;

namespace Conreign.Framework.Core
{
    internal static class ErrorFactory
    {
        public static Failure<SystemError> ServiceUnavailable()
        {
            return Failure.Create("Service is currently unavailable.", SystemError.ServiceUnavailable);
        }

        public static UserError<RoutingError> HandlerNotFound(string type)
        {
            return UserError.Create($"Handler for {type} was not found.", RoutingError.HandlerNotFound);
        }

        public static UserError<T> ForUserException<T>(UserException<T> exception)
        {
            return UserError.Create(exception);
        }

        public static Failure<string> ForAggregateException(AggregateException exception)
        {
            return Failure.Create(exception, exception.InnerException.Message);
        }

        public static Failure<string> ForException(Exception exception)
        {
            return Failure.Create(exception);
        }
    }
}