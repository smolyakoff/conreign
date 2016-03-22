namespace Conreign.Core.Contracts.Game.Data
{
    public class WelcomeMessagePayload
    {
        public string AccessToken { get; set; }

        public PlayerSettingsPayload PlayerSettings { get; set; }

        public GameRoomPayload SuggestedGameRoom { get; set; }
    }
}