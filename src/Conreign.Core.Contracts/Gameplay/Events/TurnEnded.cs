using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnEnded : IClientEvent, IRoomEvent
    {
        public TurnEnded(string roomId, Guid userId)
        {
            RoomId = roomId;
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        public string RoomId { get; }
        public Guid UserId { get; }
        public DateTime Timestamp { get; }
    }
}