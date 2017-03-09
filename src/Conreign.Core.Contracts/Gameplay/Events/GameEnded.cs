using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
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

        public string RoomId { get; }
        public Dictionary<Guid, GameStatisticsData> Statistics { get; }
        public DateTime Timestamp { get; }
    }
}