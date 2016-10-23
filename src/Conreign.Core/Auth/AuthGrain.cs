using System.Security.Claims;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Auth
{
    [StatelessWorker]
    public class AuthGrain : Grain, IAuthGrain
    {
        private AuthService _authService;

        public override Task OnActivateAsync()
        {
            var options = new AuthOptions {JwtSecret = "conreign!"};
            _authService = new AuthService(options);
            return base.OnActivateAsync();
        }

        public Task<ClaimsIdentity> Authenticate(string accessToken)
        {
            return _authService.Authenticate(accessToken);
        }

        public Task<string> Login()
        {
            return _authService.Login();
        }
    }
}