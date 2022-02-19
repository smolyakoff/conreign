using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface ILobby : IRoom
{
    Task UpdateGameOptions(Guid userId, GameOptionsData options);
    Task UpdatePlayerOptions(Guid userId, PlayerOptionsData options);
    Task<IGame> StartGame(Guid userId);
}