using System;

namespace Conreign.Server.Gameplay
{
    public class LobbyGrainOptions
    {
        public TimeSpan MaxInactivityPeriod { get; set; } = TimeSpan.FromMinutes(30);
    }
}