using Conreign.Server.Contracts.Shared.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
[Immutable]
[Persistent]
public class PlayerDead : IClientEvent, IRoomEvent
{
    public PlayerDead(string roomId, Guid userId)
    {
        RoomId = roomId;
        UserId = userId;
        Timestamp = DateTime.UtcNow;
    }

    public Guid UserId { get; }
    public DateTime Timestamp { get; }
    public string RoomId { get; }
}