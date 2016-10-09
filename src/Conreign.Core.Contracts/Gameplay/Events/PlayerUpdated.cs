using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class PlayerUpdated : IClientEvent
    {
        public PlayerData Player { get; }

        public PlayerUpdated(PlayerData player)
        {
            Player = player;
        }
    }
}
