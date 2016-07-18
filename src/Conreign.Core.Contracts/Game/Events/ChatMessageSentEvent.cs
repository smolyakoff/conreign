using System;

namespace Conreign.Core.Contracts.Game.Events
{
    public class ChatMessageSentEvent
    {
        public Guid SenderId { get; set; }
        public string Nickname { get; set; }
        public string Text { get; set; }
    }
}