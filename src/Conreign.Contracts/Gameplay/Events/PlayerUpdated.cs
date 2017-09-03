using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class PlayerUpdated : IClientEvent, IRoomEvent
    {
        public PlayerUpdated(string roomId, PlayerData player)
        {
            RoomId = roomId;
            Player = player;
            Timestamp = DateTime.UtcNow;
        }

        public PlayerData Player { get; }
        public DateTime Timestamp { get; }
        public string RoomId { get; }
    }
}