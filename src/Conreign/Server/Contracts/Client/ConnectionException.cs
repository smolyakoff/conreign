using System.Runtime.Serialization;
using Conreign.Server.Contracts.Shared.Errors;

namespace Conreign.Server.Contracts.Client;

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