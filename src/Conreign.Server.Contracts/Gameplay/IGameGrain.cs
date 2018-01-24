using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IGameGrain : IGrainWithStringKey, IGame
    {
    }
}