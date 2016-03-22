using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Conreign.Framework.Contracts.Core
{
    [Serializable]
    public class UserException<T> : UserException
    {
        public UserException(string message, T type) : this(message, type, (string) null)
        {
        }

        public UserException(string message, T type, Exception inner) : this(message, type, null, inner)
        {
        }

        public UserException(string message, T type, string title) : base(message)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message should not be empty.", nameof(message));
            }
            Type = type;
            Title = title;
        }

        public UserException(string message, T type, string title, Exception inner) : base(message, inner)
        {
            if (string.IsNullOrEmpty(message))
            {
                throw new ArgumentException("Message should not be empty.", nameof(message));
            }
            if (inner == null)
            {
                throw new ArgumentNullException(nameof(inner));
            }
            Type = type;
            Title = title;
        }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Title = info.GetString("Title");
            Type = (T) info.GetValue("Type", typeof(T));
        }

        public string Title { get; }

        public T Type { get; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            info.AddValue("Type", Type);
            info.AddValue("Title", Title);
            base.GetObjectData(info, context);
        }
    }
}