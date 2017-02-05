using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    [Persistent]
    public class PlayerDead : IClientEvent, IRoomEvent
    {
        public PlayerDead(string roomId, Guid userId)
        {
            RoomId = roomId;
            UserId = userId;
        }

        public Guid UserId { get; }
        public string RoomId { get; }
    }
}