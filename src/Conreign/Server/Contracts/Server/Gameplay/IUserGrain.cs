using Conreign.Server.Contracts.Shared.Gameplay;
using Orleans;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IUserGrain : IGrainWithGuidKey, IUser
{
}