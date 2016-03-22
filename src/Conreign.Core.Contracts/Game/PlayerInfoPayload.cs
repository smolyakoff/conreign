using System.Collections.Generic;
using Conreign.Core.Contracts.Game.Data;

namespace Conreign.Core.Contracts.Game
{
    public class PlayerInfoPayload
    {
        public PlayerSettingsPayload Settings { get; set; }

        public HashSet<string> ConnectionIds { get; set; }
    }
}