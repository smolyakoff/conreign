using System;

namespace Conreign.Framework.Contracts.Core.Data
{
    public class UserError<T, TData> : UserError<T>
    {
        public UserError(UserException<T, TData> exception) : base(exception)
        {
            Info = exception.Info;
        }

        public UserError(string message, T type, Exception exception = null, string title = null, TData info = default(TData)) 
            : base(message, type, exception, title)
        {
            Info = info;
        }

        public TData Info { get; }
    }
}
