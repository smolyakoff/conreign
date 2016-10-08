using System.Collections.Generic;
using Conreign.Core.Communication;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class LobbyState
    {
        public bool IsGameStarted { get; set; }
        public HubState Hub { get; set; } = new HubState();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public MapData Map { get; set; } = new MapData();
    }
}