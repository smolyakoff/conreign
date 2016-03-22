using System;
using System.Collections.Generic;
using Orleans;

namespace Conreign.Core.Game.Data
{
    public class PlayerData : GrainState
    {
        public PlayerData()
        {
            Connections = new HashSet<ConnectionData>();
            CreatedAt = DateTime.UtcNow;
        }

        public Guid PlayerKey { get; set; }

        public DateTime CreatedAt { get; set; }

        public PlayerSettingsData Settings { get; set; }

        public HashSet<ConnectionData> Connections { get; set; }
    }
}