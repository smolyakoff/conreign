using System;
using System.Runtime.Serialization;
using Conreign.Contracts.Errors;

namespace Conreign.Client.Contracts
{
    [Serializable]
    public class ConnectionException : ConreignException
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