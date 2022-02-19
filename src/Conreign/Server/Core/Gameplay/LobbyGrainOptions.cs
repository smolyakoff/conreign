namespace Conreign.Server.Core.Gameplay;

public class LobbyGrainOptions
{
    public TimeSpan MaxInactivityPeriod { get; set; } = TimeSpan.FromMinutes(30);
}