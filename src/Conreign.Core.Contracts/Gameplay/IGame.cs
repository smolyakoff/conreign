using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IGame : IRoom
    {
        Task LaunchFleet();
        Task EndTurn();
    }
}