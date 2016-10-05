using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Commands;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface ILobby : IRoom
    {
        Task UpdateGameSettings();
        Task UpdatePlayerOptions(UpdatePlayerOptionsCommand command);
        Task<IGame> StartGame(StartGameCommand command);
    }
}