using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface ILobby : IRoom
    {
        Task UpdateGameOptions(Guid userId, GameOptionsData options);
        Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options);
        Task StartGame(Guid userId);
    }
}