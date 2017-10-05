using System;

namespace Conreign.Server.Gameplay
{
    public class GameGrainOptions
    {
        public int TurnLengthInTicks { get; set; } = 12;
        public TimeSpan TickInterval { get; set; } = TimeSpan.FromSeconds(5);
        public TimeSpan MaxInactivityPeriod { get; set; } = TimeSpan.FromMinutes(30);
    }
}