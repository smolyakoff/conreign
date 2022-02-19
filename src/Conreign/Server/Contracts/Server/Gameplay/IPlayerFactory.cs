using Conreign.Server.Contracts.Shared.Gameplay;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IPlayerFactory
{
    Task<IPlayer> CreatePlayer(string roomId);
}