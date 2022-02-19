using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Core.Gameplay;

public class PlayerGameState
{
    public GameStatisticsData Statistics { get; set; } = new();
    public List<FleetData> WaitingFleets { get; set; } = new();
    public List<MovingFleetData> MovingFleets { get; set; } = new();
    public TurnStatus TurnStatus { get; set; } = TurnStatus.Thinking;
}