using Orleans;

namespace Conreign.Server.Contracts.Auth
{
    public interface IAuthGrain : IGrainWithIntegerKey, IAuthService
    {
    }
}