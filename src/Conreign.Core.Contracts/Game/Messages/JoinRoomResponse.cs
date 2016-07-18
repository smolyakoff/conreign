namespace Conreign.Core.Contracts.Game.Messages
{
    public class JoinRoomResponse
    {
        public bool IsAvailable { get; set; }

        public PlayerRole? Role { get; set; }
    }
}
