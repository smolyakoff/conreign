namespace Conreign.Core.Gameplay.Battle
{
    public interface IBattleStrategy
    {
        BattleOutcome Calculate(BattleFleet attacker, BattleFleet defender);
    }
}
