using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IPlayerFactory
    {
        Task<IPlayer> CreatePlayer(string roomId);
    }
}