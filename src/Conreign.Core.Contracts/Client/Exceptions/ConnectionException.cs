using System;
using System.Runtime.Serialization;

namespace Conreign.Core.Contracts.Client.Exceptions
{
    [Serializable]
    public class ConnectionException : ClientException
    {
        public ConnectionException()
        {
        }

        public ConnectionException(string message) : base(message)
        {
        }

        public ConnectionException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConnectionException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}