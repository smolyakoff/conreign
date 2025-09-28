namespace Conreign.Server.Contracts.Shared.Errors;

public static class UserExceptionExtensions
{
    public static UserError ToUserError(this UserException exception)
    {
        return UserError.Create(exception);
    }

    public static UserError<T> ToUserError<T>(this UserException<T> exception) where T : struct
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return UserError.Create(exception);
    }

    public static UserError<T, TDetails> ToUserError<T, TDetails>(this UserException<T, TDetails> exception)
        where T : struct
    {
        if (exception == null)
        {
            throw new ArgumentNullException(nameof(exception));
        }

        return UserError.Create(exception);
    }
}