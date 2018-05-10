using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class GameEnded : IClientEvent, IServerEvent
    {
        public GameEnded(Dictionary<Guid, GameStatisticsData> statistics)
        {
            Statistics = statistics;
            Timestamp = DateTime.UtcNow;
        }

        public Dictionary<Guid, GameStatisticsData> Statistics { get; }
        public DateTime Timestamp { get; }
    }
}