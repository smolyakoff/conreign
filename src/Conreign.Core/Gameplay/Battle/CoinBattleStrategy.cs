using System;

namespace Conreign.Core.Gameplay.Battle
{
    public class CoinBattleStrategy : IBattleStrategy
    {
        private readonly Random _random = new Random();

        public BattleOutcome Calculate(BattleFleet attacker, BattleFleet defender)
        {
            if (attacker == null)
            {
                throw new ArgumentNullException(nameof(attacker));
            }
            if (defender == null)
            {
                throw new ArgumentNullException(nameof(defender));
            }
            var totalPower = attacker.Power + defender.Power;
            var attackerShips = attacker.Ships;
            var defenderShips = defender.Ships;
            var attackerWinThreshold = attacker.Power/totalPower; 

            while (attackerShips > 0 && defenderShips > 0)
            {
                var r = _random.NextDouble();
                if (r < attackerWinThreshold)
                {
                    defenderShips -= 1;
                }
                else
                {
                    attackerShips -= 1;
                }
            }
            return new BattleOutcome(attackerShips, defenderShips);
        }
    }
}
