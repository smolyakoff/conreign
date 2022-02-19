using System.Runtime.Serialization;

namespace Conreign.Server.Contracts.Shared.Errors;

[Serializable]
public class UserException<T, TDetails> : UserException<T> where T : struct
{
    public UserException(T type, TDetails details, string message = null) : base(type, message)
    {
        if (details == null)
        {
            throw new ArgumentNullException(nameof(details));
        }

        Details = details;
    }

    protected UserException(
        SerializationInfo info,
        StreamingContext context) : base(info, context)
    {
        Details = (TDetails)info.GetValue("Details", typeof(TDetails));
    }

    public TDetails Details { get; }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
        base.GetObjectData(info, context);
        info.AddValue("Details", Details);
    }
}