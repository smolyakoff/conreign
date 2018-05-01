using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IPlayerGrain : IGrainWithGuidCompoundKey, IPlayer
    {
    }
}