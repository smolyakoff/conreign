using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth;
using Conreign.Core.Contracts.Auth.Data;
using Orleans;
using Orleans.Concurrency;

namespace Conreign.Core.Auth
{
    [StatelessWorker]
    public class AuthGrain : Grain, IAuthGrain
    {
        public Task<AuthenticationResultPayload> AuthenticateAnonymousUser()
        {
            throw new System.NotImplementedException();
        }
    }
}
