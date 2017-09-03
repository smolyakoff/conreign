using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
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
}