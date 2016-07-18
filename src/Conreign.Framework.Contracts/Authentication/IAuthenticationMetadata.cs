using System.Security.Claims;

namespace Conreign.Framework.Contracts.Authentication
{
    public interface IAuthenticationMetadata
    {
        string AccessToken { get; }
        ClaimsPrincipal Principal { set; get; }
        AuthenticationError? AuthenticationError { set; get; }
    }
}