using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    [Persistent]
    public class ChatMessageReceived : IClientEvent, IRoomEvent
    {
        public ChatMessageReceived(string roomId, Guid senderId, TextMessageData message)
        {
            SenderId = senderId;
            Message = message;
            RoomId = roomId;
            Timestamp = DateTime.UtcNow;
        }

        public Guid SenderId { get; }
        public TextMessageData Message { get; }
        public DateTime Timestamp { get; }
        public string RoomId { get; }
    }
}