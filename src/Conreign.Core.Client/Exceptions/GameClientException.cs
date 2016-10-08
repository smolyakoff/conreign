using System;
using System.Runtime.Serialization;

namespace Conreign.Core.Client.Exceptions
{
    [Serializable]
    public class GameClientException : Exception
    {
        public GameClientException()
        {
        }

        public GameClientException(string message) : base(message)
        {
        }

        public GameClientException(string message, Exception inner) : base(message, inner)
        {
        }

        protected GameClientException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
