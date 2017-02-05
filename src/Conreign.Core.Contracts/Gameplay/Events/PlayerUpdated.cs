using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class PlayerUpdated : IClientEvent, IRoomEvent
    {
        public PlayerUpdated(string roomId, PlayerData player)
        {
            RoomId = roomId;
            Player = player;
        }

        public string RoomId { get; }
        public PlayerData Player { get; }
    }
}