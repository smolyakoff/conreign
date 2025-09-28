using System.Security.Claims;

namespace Conreign.Server.Contracts.Server.Auth;

public interface IAuthService
{
    Task<ClaimsIdentity> Authenticate(string accessToken);
    Task<string> Login(string accessToken = null);
}