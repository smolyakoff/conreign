using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
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

        public string RoomId { get; }
        public int Turn { get; }
        public DateTime Timestamp { get; }
    }
}