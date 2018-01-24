using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    [Persistent]
    public class PlayerDead : IRoomEvent, IClientEvent, IServerEvent
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
}