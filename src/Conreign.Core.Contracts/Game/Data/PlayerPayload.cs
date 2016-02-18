using System;

namespace Conreign.Core.Contracts.Game.Data
{
    public class PlayerPayload
    {
        public Guid PlayerKey { get; set; }

        public string Token { get; set; }

        public PlayerSettingsPayload Settings { get; set; }
    }
}