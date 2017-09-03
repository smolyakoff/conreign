using System;
using System.Runtime.Serialization;

namespace Conreign.Contracts.Errors
{
    [Serializable]
    public class ServerException : ConreignException
    {
        public ServerException()
        {
        }

        public ServerException(Exception inner) : base(inner.Message, inner)
        {
        }

        protected ServerException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}