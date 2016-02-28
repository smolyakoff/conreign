using Conreign.Core.Contracts.Abstractions;

namespace Conreign.Core.Contracts.Auth
{
    public class AuthMeta
    {
        public string AccessToken { get; set; }

        public UserMessage ErrorMessage { get; set; }

        public bool IsAuthenticated => ErrorMessage == null;
    }
}
