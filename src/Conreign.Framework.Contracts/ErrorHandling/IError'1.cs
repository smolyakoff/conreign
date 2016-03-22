namespace Conreign.Framework.Contracts.Core.Data
{
    public interface IError<out T> : IError
    {
        T Type { get; }
    }
}
