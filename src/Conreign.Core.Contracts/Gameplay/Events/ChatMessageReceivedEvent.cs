using System;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    public class ChatMessageReceivedEvent
    {
        public Guid SenderId { get; set; }
        public string Text { get; set; }
    }
}