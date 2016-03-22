namespace Conreign.Core.Contracts.Auth
{
    public class AuthMeta
    {
        public string AccessToken { get; set; }

        public AuthenticationError? Error { get; set; }

        public bool IsAuthenticated => Error == null;
    }
}