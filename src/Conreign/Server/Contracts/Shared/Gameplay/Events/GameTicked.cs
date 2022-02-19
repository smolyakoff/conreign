using Conreign.Server.Contracts.Shared.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Immutable]
[Serializable]
public class GameTicked : IClientEvent, IRoomEvent
{
    public GameTicked(string roomId, int tick)
    {
        Tick = tick;
        RoomId = roomId;
        Timestamp = DateTime.UtcNow;
    }

    public int Tick { get; }
    public DateTime Timestamp { get; }
    public string RoomId { get; }
}