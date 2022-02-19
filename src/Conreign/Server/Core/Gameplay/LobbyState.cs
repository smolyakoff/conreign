using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Conreign.Server.Core.Editor;
using Conreign.Server.Core.Presence;

namespace Conreign.Server.Core.Gameplay;

public class LobbyState
{
    public string RoomId { get; set; }
    public bool IsGameStarted { get; set; }
    public HubState Hub { get; set; } = new();
    public List<PlayerData> Players { get; set; } = new();
    public MapEditorState MapEditor { get; set; } = new();
}