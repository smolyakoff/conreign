using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
[Immutable]
public class TurnCalculationEnded : IClientEvent, IRoomEvent
{
    public TurnCalculationEnded(string roomId, int turn, MapData map, List<MovingFleetData> movingFleets)
    {
        RoomId = roomId;
        Turn = turn;
        Map = map;
        MovingFleets = movingFleets;
        Timestamp = DateTime.UtcNow;
    }

    public int Turn { get; }
    public MapData Map { get; }
    public List<MovingFleetData> MovingFleets { get; }
    public DateTime Timestamp { get; }

    public string RoomId { get; }
}