using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface ILobby : IRoom
    {
        Task UpdateGameOptions(Guid userId, GameOptionsData options);
        Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options);
        Task<IGame> StartGame(Guid userId);
    }
}