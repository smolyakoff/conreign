using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface ILobbyGrain : 
        IGrainWithStringKey,
        ILobby,
        IEventHandler<GameEnded>
    {
    }
}