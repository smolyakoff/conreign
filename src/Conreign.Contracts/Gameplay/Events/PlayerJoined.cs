using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Obsolete("Use PlayerListChanged instead.")]
    public class PlayerJoined : IClientEvent
    {
        public PlayerJoined(PlayerData player)
        {
            Player = player;
            Timestamp = DateTime.UtcNow;
        }

        public PlayerData Player { get; }
        public DateTime Timestamp { get; }
    }
}