using System;
using System.Runtime.Serialization;

namespace Conreign.Core.Contracts.Exceptions
{
    [Serializable]
    public class UserException<T> : Exception where T : struct
    {
        public UserException(T type, string message = null) : base(message ?? $"[{type.GetType().Name}]: {type}.")
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("User error type should be represented as enum.", nameof(type));
            }
            Type = type;
        }

        public T Type { get; }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Type = (T)info.GetValue("Type", typeof(T));
        }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
            info.AddValue("Type", Type);
        }
    }
}
