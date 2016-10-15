using System;

namespace Conreign.Core.Gameplay.Battle
{
    public class BattleOutcome
    {
        public BattleOutcome(int attackerShips, int defenderShips)
        {
            if (attackerShips < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(attackerShips));
            }
            if (defenderShips < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(defenderShips));
            }
            if (attackerShips == 0 && defenderShips == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(defenderShips));
            }
            AttackerShips = attackerShips;
            DefenderShips = defenderShips;
        }

        public int AttackerShips { get; }
        public int DefenderShips { get; }
    }
}