using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayerGrain : IGrainWithGuidCompoundKey, IConnectablePlayer, IEventHandler<GameStarted.System>
    {
    }
}