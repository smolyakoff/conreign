using Conreign.Server.Contracts.Shared.Communication;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
public class GameStarted : IClientEvent
{
    public GameStarted()
    {
        Timestamp = DateTime.UtcNow;
    }

    public DateTime Timestamp { get; }
}