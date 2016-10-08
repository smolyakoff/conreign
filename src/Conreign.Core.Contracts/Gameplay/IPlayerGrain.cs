using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayerGrain : IGrainWithGuidCompoundKey, IConnectablePlayer
    {
    }
}