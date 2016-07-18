using System.Security.Claims;

namespace Conreign.Framework.Contracts.Authentication
{
    public class AuthenticateResponse
    {
        public ClaimsPrincipal Principal { get; set; }
        public AuthenticationError? Error { get; set; }
    }
}