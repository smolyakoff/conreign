using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IGameFactory
    {
        Task<IGame> CreateGame();
    }
}
