using System;
using System.Runtime.Serialization;

namespace Conreign.Core.Contracts.Exceptions
{
    [Serializable]
    public abstract class UserException : Exception
    {
        internal UserException(string message) : base(message)
        {
        }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }

        public static UserException<T> Create<T>(T type, string message = null) where T : struct
        {
            return new UserException<T>(type, message);
        }

        public static UserException<T, TDetails> Create<T, TDetails>(T type, TDetails details, string message = null) where T : struct
        {
            return new UserException<T, TDetails>(type, details, message);
        }
    }
}
