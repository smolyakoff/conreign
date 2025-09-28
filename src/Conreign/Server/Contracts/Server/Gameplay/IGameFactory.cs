namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IGameFactory
{
    Task<IGame> CreateGame(Guid userId);
}