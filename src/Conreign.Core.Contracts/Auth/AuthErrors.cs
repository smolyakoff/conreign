using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Core.Contracts.Auth
{
    public static class AuthErrors
    {
        public static readonly UserMessage TokenExpired = 
            UserMessage.FromResource(() => Errors.TokenExpiredMessage);

        public static readonly UserMessage BadToken = 
            UserMessage.FromResource(() => Errors.BadTokenMessage);
    }
}
