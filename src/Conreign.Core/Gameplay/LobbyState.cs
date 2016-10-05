using System.Collections.Generic;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class LobbyState
    {
        public bool IsGameStarted { get; set; }
        public HubState Hub { get; set; }
        public List<PlayerOptionsState> Players { get; set; } = new List<PlayerOptionsState>();
        public MapState Map { get; set; }
    }
}