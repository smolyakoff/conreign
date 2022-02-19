using System.Runtime.Serialization;

namespace Conreign.Server.Contracts.Shared.Errors;

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