using System;

namespace Conreign.Server.Gameplay
{
    public class GameStalledTurnOutcome : IGameTurnOutcome
    {
        public GameStalledTurnOutcome(TimeSpan inactivityPeriod)
        {
            if (inactivityPeriod.Ticks < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(inactivityPeriod));
            }
            InactivityPeriod = inactivityPeriod;
        }

        public TimeSpan InactivityPeriod { get; }
    }
}