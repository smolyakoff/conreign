using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;
using Conreign.Core.Editor;
using Conreign.Server.Presence;

namespace Conreign.Server.Gameplay
{
    public class LobbyState
    {
        public string RoomId { get; set; }
        public bool IsGameStarted { get; set; }
        public HubState Hub { get; set; } = new HubState();
        public List<PlayerData> Players { get; set; } = new List<PlayerData>();
        public MapEditorState MapEditor { get; set; } = new MapEditorState();
    }
}