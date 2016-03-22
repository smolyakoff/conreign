namespace Conreign.Core.Contracts.Game.Data
{
    public class GameRoomPayload
    {
        public string Permalink { get; set; }

        public string Name { get; set; }

        public GameRoomStatus Status { get; set; }
    }
}