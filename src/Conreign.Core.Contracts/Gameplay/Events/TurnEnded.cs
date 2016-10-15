using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class TurnEnded : IClientEvent
    {
        public TurnEnded(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; }
    }
}