using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay.Data;
using Conreign.Core.Gameplay.Editor;
using Conreign.Core.Presence;

namespace Conreign.Core.Gameplay
{
    public class LobbyState
    {
        public string RoomId { get; set; }
        public bool IsGameStarted { get; set; }
        public HubState Hub { get; } = new HubState();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public MapEditorState MapEditor { get; set; } = new MapEditorState();
    }
}