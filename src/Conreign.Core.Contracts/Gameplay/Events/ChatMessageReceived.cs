using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
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
        }

        public Guid SenderId { get; }
        public TextMessageData Message { get; }
    }
}