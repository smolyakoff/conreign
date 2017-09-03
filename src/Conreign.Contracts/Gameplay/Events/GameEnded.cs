using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class GameEnded : IClientEvent, IServerEvent, IRoomEvent
    {
        public GameEnded(string roomId, Dictionary<Guid, GameStatisticsData> statistics)
        {
            RoomId = roomId;
            Statistics = statistics;
            Timestamp = DateTime.UtcNow;
        }

        public Dictionary<Guid, GameStatisticsData> Statistics { get; }
        public DateTime Timestamp { get; }

        public string RoomId { get; }
    }
}