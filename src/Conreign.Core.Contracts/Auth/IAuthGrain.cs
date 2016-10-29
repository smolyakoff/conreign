using Orleans;

namespace Conreign.Core.Contracts.Auth
{
    public interface IAuthGrain : IGrainWithIntegerKey, IAuthService
    {
    }
}