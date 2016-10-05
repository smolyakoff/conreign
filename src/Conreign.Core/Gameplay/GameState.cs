using System;
using System.Collections.Generic;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class GameState
    {
        public int Turn { get; set; }
        public MapState Map { get; set; } = new MapState();
        public List<PlayerOptionsState> Players { get; set; } = new List<PlayerOptionsState>();
        public HubState Hub { get; set; } = new HubState();
        public Dictionary<Guid, GameStatisticsState> Statistics { get; set; } = new Dictionary<Guid, GameStatisticsState>();
    }
}
