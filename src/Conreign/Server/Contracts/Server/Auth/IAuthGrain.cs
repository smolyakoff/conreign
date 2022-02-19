using Orleans;

namespace Conreign.Server.Contracts.Server.Auth;

public interface IAuthGrain : IGrainWithIntegerKey, IAuthService
{
}