using System;
using System.Runtime.Serialization;

namespace Conreign.Framework.Contracts.Core
{
    [Serializable]
    public class UserException : Exception
    {
        protected UserException()
        {
        }

        protected UserException(string message, Exception inner) : base(message, inner)
        {
        }

        protected UserException(string message) : base(message)
        {
        }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
