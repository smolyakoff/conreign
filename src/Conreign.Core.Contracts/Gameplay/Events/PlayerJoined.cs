using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class PlayerJoined : IClientEvent
    {
        public PlayerData Player { get; set; }
    }
}
