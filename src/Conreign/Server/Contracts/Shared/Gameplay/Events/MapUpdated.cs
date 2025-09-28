using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
[Immutable]
public class MapUpdated : IClientEvent, IRoomEvent
{
    public MapUpdated(string roomId, MapData map)
    {
        RoomId = roomId;
        Map = map;
        Timestamp = DateTime.UtcNow;
    }

    public MapData Map { get; }
    public DateTime Timestamp { get; }

    public string RoomId { get; }
}