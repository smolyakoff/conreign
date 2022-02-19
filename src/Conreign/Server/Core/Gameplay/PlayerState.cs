using Conreign.Server.Contracts.Server.Gameplay;

namespace Conreign.Server.Core.Gameplay;

public class PlayerState
{
    public Guid UserId { get; set; }
    public string RoomId { get; set; }
    public ILobby Lobby { get; set; }
    public IGame Game { get; set; }
    public IRoom Room => Game as IRoom ?? Lobby;
}