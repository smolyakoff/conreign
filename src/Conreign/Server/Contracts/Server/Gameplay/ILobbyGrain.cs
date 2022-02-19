using Conreign.Server.Contracts.Server.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Events;
using Orleans;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface ILobbyGrain : IGrainWithStringKey,
    ILobby,
    IGameFactory,
    IEventHandler<GameEnded>
{
}