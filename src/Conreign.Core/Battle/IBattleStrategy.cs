namespace Conreign.Core.Battle
{
    public interface IBattleStrategy
    {
        BattleOutcome Calculate(BattleFleet attacker, BattleFleet defender);
    }
}