using System.Threading.Tasks;
using Conreign.Core.Contracts.Auth.Actions;
using Conreign.Core.Contracts.Auth.Data;
using Orleans;

namespace Conreign.Core.Contracts.Auth
{
    public interface IAuthGrain : IGrainWithIntegerKey
    {
        Task<AccessTokenPayload> LoginAnonymous(LoginAnonymousAction action);

        Task<AuthenticationStatusPayload> Authenticate(AuthenticateAction action);
    }
}
