using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class PlayerJoined : IClientEvent
    {
        public PlayerJoined(PlayerData player)
        {
            Player = player;
        }

        public PlayerData Player { get; }
    }
}
