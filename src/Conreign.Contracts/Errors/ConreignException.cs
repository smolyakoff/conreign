using System;
using System.Runtime.Serialization;

namespace Conreign.Contracts.Errors
{
    [Serializable]
    public class ConreignException : Exception
    {
        public ConreignException()
        {
        }

        public ConreignException(string message) : base(message)
        {
        }

        public ConreignException(string message, Exception inner) : base(message, inner)
        {
        }

        protected ConreignException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
        }
    }
}