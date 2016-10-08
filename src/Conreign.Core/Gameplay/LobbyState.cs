using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Presence;

namespace Conreign.Core.Gameplay
{
    public class LobbyState
    {
        public bool IsGameStarted { get; set; }
        public HubState Hub { get; set; } = new HubState();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public MapData Map { get; set; } = new MapData();
        public GameOptionsData GameOptions { get; set; } = new GameOptionsData();
    }
}