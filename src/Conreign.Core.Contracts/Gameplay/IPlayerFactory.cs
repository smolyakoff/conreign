using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayerFactory
    {
        Task<IPlayer> CreatePlayer(string roomId);
    }
}