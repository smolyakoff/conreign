using System.Security.Claims;
using System.Threading.Tasks;

namespace Conreign.Server.Contracts.Auth
{
    public interface IAuthService
    {
        Task<ClaimsIdentity> Authenticate(string accessToken);
        Task<string> Login(string accessToken = null);
    }
}