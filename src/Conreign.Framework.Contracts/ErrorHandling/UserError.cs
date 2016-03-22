namespace Conreign.Framework.Contracts.Core.Data
{
    public class UserError
    {
        protected UserError()
        { 
        }

        public static UserError<T> Create<T>(string message, T type)
        {
            return new UserError<T>(message, type);
        }

        public static UserError<object> Create(UserException exception)
        {
            dynamic value = exception;
            return Create(value);
        }

        public static UserError<T> Create<T>(UserException<T> exception)
        {
            return new UserError<T>(exception);
        }

        public static UserError<T, TData> Create<T, TData>(UserException<T, TData> exception)
        {
            return new UserError<T, TData>(exception);
        }
    }
}
