using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnCalculationStarted : IClientEvent, IRoomEvent
    {
        public TurnCalculationStarted(string roomId, int turn)
        {
            Turn = turn;
            RoomId = roomId;
        }

        public string RoomId { get; }
        public int Turn { get; }
    }
}