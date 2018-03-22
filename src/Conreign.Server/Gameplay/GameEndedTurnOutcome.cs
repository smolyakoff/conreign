using System;

namespace Conreign.Server.Gameplay
{
    public class GameEndedTurnOutcome : IGameTurnOutcome
    {
        public int TurnsCount { get; }

        public GameEndedTurnOutcome(int turnsCount)
        {
            if (turnsCount <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(turnsCount));
            }
            TurnsCount = turnsCount;
        }
    }
}