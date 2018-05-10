using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    public class PlayerUpdated : IClientEvent
    {
        public PlayerUpdated(PlayerData player)
        {
            Player = player;
            Timestamp = DateTime.UtcNow;
        }

        public PlayerData Player { get; }
        public DateTime Timestamp { get; }
    }
}