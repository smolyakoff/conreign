using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Gameplay
{
    public class PlayerGameState
    {
        public GameStatisticsData Statistics { get; set; } = new GameStatisticsData();
        public List<FleetData> WaitingFleets { get; set; } = new List<FleetData>();
        public List<MovingFleetData> MovingFleets { get; set; } = new List<MovingFleetData>();
        public TurnStatus TurnStatus { get; set; } = TurnStatus.Thinking;
    }
}