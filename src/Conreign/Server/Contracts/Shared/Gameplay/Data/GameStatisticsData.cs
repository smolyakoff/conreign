namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

public class GameStatisticsData
{
    public int? DeathTurn { get; set; }
    public int ShipsProduced { get; set; }
    public int BattlesWon { get; set; }
    public int BattlesLost { get; set; }
    public int ShipsLost { get; set; }
    public int ShipsDestroyed { get; set; }
}