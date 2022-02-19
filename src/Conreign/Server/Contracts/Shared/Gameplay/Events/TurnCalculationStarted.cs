using Conreign.Server.Contracts.Shared.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
[Immutable]
public class TurnCalculationStarted : IClientEvent, IRoomEvent
{
    public TurnCalculationStarted(string roomId, int turn)
    {
        Turn = turn;
        RoomId = roomId;
        Timestamp = DateTime.UtcNow;
    }

    public int Turn { get; }
    public DateTime Timestamp { get; }

    public string RoomId { get; }
}