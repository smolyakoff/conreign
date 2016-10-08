using System;
using System.Collections.Generic;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Contracts.Presence;
using Conreign.Core.Presence;

namespace Conreign.Core.Gameplay
{
    public class GameState
    {
        public string RoomId { get; set; }
        public int Turn { get; set; }
        public MapData Map { get; set; } = new MapData();
        public List<GamePlayerState> Players { get; set; } = new List<GamePlayerState>();
        public HubState Hub { get; set; } = new HubState();
        public Dictionary<Guid, GameStatisticsState> Statistics { get; set; } = new Dictionary<Guid, GameStatisticsState>();
    }
}
