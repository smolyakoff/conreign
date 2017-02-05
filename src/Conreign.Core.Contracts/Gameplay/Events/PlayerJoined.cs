using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class PlayerJoined : IClientEvent, IRoomEvent
    {
        public PlayerJoined(string roomId, PlayerData player)
        {
            RoomId = roomId;
            Player = player;
        }

        public PlayerData Player { get; }
        public string RoomId { get; }
    }
}