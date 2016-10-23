using System.Security.Claims;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Auth
{
    public interface IAuthService
    {
        Task<ClaimsIdentity> Authenticate(string accessToken);
        Task<string> Login();
    }
}
