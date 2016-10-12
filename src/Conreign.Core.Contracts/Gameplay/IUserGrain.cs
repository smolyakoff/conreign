using Conreign.Core.Contracts.Communication;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IUserGrain : IGrainWithGuidKey, IUser, IPlayerFactory, ISystemPublisherFactory
    {
    }
}