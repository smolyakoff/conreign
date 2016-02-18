namespace Conreign.Core.Contracts.Game.Data
{
    public class GameParametersPayload
    {
        public string GameKey { get; set; }

        public int MapWidth { get; set; }

        public int MapHeight { get; set; }

        public int NeutralPlayersCount { get; set; }

        public int BotsCount { get; set; }
    }
}