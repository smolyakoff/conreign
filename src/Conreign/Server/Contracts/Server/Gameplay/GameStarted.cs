using Conreign.Server.Contracts.Shared.Communication;

namespace Conreign.Server.Contracts.Server.Gameplay;

[Serializable]
public class GameStartedServer : IServerEvent
{
    public GameStartedServer(IGame game)
    {
        Game = game;
    }

    public IGame Game { get; }
}