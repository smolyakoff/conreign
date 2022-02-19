using System.Runtime.Serialization;

namespace Conreign.Server.Contracts.Shared.Errors;

[Serializable]
public class UserException<T> : UserException where T : struct
{
    public UserException(T type, string message = null) : base(message ?? $"[{type.GetType().Name}]: {type}.")
    {
        if (!typeof(T).IsEnum)
        {
            throw new ArgumentException("User error type should be represented as enum.", nameof(type));
        }

        Type = type;
    }

    protected UserException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
        Type = (T)info.GetValue("Type", typeof(T));
    }

    public T Type { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Type", Type);
    }
}