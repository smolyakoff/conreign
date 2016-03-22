using System;
using Conreign.Framework.Contracts.ErrorHandling;

namespace Conreign.Framework.Contracts.Core.Data
{
    public static class Failure
    {
        public static Failure<T> Create<T>(string message, T type)
        {
            return new Failure<T>(message, type);
        }

        public static Failure<string> Create(Exception exception, string message)
        {
            return new Failure<string>(message, exception.GetType().Name.Replace("Exception", string.Empty));
        }

        public static Failure<string> Create(Exception exception)
        {
            return Create(exception, exception.Message);
        } 
    }
}
