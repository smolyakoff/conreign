namespace Conreign.Core.Contracts.Game.Data
{
    public class PlayerPayload
    {
        public string AccessToken { get; set; }

        public PlayerSettingsPayload Settings { get; set; }
    }
}