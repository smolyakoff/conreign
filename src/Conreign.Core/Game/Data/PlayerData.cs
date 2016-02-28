using System;
using System.Collections.Generic;

namespace Conreign.Core.Game.Data
{
    public class PlayerData
    {
        public PlayerData()
        {
            ConnectionIds = new HashSet<string>();
        }

        public Guid PlayerKey { get; set; }

        public PlayerSettingsData Settings { get; set; }

        public HashSet<string> ConnectionIds { get; set; } 
    }
}