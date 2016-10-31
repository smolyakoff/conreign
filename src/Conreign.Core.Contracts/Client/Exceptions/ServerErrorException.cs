using System;
using System.Runtime.Serialization;

namespace Conreign.Core.Contracts.Client.Exceptions
{
    [Serializable]
    public class ServerErrorException : ClientException
    {
        public ServerErrorException()
        {
        }

        public ServerErrorException(Exception inner) : base(inner.Message, inner)
        {
        }

        protected ServerErrorException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}
