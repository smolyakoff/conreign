using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface ILobbyGrain : IGrainWithStringKey, 
        ILobby, 
        IGameFactory, 
        IEventHandler<GameEnded>
    {
    }
}
