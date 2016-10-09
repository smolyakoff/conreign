using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class ChatMessageReceived : IClientEvent
    {
        public Guid SenderId { get; set; }
        public string Text { get; set; }
    }
}