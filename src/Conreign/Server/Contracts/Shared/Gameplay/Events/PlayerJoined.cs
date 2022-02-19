using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
public class PlayerJoined : IClientEvent, IRoomEvent
{
    public PlayerJoined(string roomId, PlayerData player)
    {
        RoomId = roomId;
        Player = player;
        Timestamp = DateTime.UtcNow;
    }

    public PlayerData Player { get; }
    public DateTime Timestamp { get; }
    public string RoomId { get; }
}