using Orleans;

namespace Conreign.Core.Contracts.Game
{
    public interface IUserBucketGrain : IGrainWithStringKey, IPlayerMembersipProvider
    {
    }
}
