using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IGameGrain : IGrainWithStringKey, IGame
    {
        Task Initialize(InitialGameData data);
    }
}