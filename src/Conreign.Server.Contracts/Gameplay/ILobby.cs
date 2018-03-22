using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface ILobby : IRoom, IEventHandler<GameEnded>
    {
        Task UpdateGameOptions(Guid userId, GameOptionsData options);
        Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options);
        Task<InitialGameData> InitializeGame(Guid userId);
    }
}