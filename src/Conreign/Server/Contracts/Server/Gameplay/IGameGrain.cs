using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Orleans;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IGameGrain : IGrainWithStringKey, IGame
{
    Task Initialize(InitialGameData data);
}