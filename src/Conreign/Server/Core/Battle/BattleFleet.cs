namespace Conreign.Server.Core.Battle;

public class BattleFleet
{
    public BattleFleet(int ships, double power)
    {
        if (ships <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(ships));
        }

        if (power <= 0 && power >= 1)
        {
            throw new ArgumentOutOfRangeException(nameof(power));
        }

        Ships = ships;
        Power = power;
    }

    public int Ships { get; }
    public double Power { get; }
}