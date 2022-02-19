namespace Conreign.Server.Core.Battle;

public interface IBattleStrategy
{
    BattleOutcome Calculate(BattleFleet attacker, BattleFleet defender);
}