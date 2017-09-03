namespace Conreign.Contracts.Errors
{
    public class UserError<T> : UserError where T : struct
    {
        public UserError(string message, T type) : base(message)
        {
            Type = type;
        }

        public T Type { get; }
        public string Category => Type.GetType().Name;
        public string Code => Type.ToString();
    }
}