using System;
using System.Runtime.Serialization;
using System.Security.Permissions;

namespace Conreign.Framework.Contracts.Core
{
    [Serializable]
    public class UserException<T, TData> : UserException<T>
    {
        public UserException(string message, T type, TData info = default(TData))
            : this(message, type, (string) null, info)
        {
        }

        public UserException(string message, T type, Exception inner, TData info = default(TData))
            : this(message, type, null, inner, info)
        {
        }

        public UserException(string message, T type, string title, TData info = default(TData))
            : base(message, type, title)
        {
            Info = info;
        }

        public UserException(string message, T type, string title, Exception inner, TData info = default(TData))
            : base(message, type, title, inner)
        {
            Info = info;
        }

        protected UserException(
            SerializationInfo info,
            StreamingContext context) : base(info, context)
        {
            Info = (TData) info.GetValue("Info", typeof (TData));
        }

        public TData Info { get; }

        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }
            info.AddValue("Info", Info);
            base.GetObjectData(info, context);
        }
    }
}