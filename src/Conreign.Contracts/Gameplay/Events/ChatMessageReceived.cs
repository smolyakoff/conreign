using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    [Persistent]
    public class ChatMessageReceived : IClientEvent
    {
        public ChatMessageReceived(Guid senderId, TextMessageData message)
        {
            SenderId = senderId;
            Message = message;
            Timestamp = DateTime.UtcNow;
        }

        public Guid SenderId { get; }
        public TextMessageData Message { get; }
        public DateTime Timestamp { get; }
    }
}