using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IGameGrain : IGrainWithStringKey, IGame
    {
        Task Initialize(InitialGameData data);
    }
}