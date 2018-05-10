using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    [Persistent]
    public class PlayerDead : IClientEvent, IServerEvent
    {
        public PlayerDead(Guid userId)
        {
            UserId = userId;
            Timestamp = DateTime.UtcNow;
        }

        public Guid UserId { get; }
        public DateTime Timestamp { get; }
    }
}