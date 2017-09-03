using Conreign.Contracts.Gameplay;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IUserGrain : IGrainWithGuidKey, IUser
    {
    }
}