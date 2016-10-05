using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Commands;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IGameGrain : IGrainWithStringKey, IGame
    {
        Task Initialize(InitializeGameCommand command);
    }
}
