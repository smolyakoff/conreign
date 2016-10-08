using System;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    public class ChatMessageReceived
    {
        public Guid SenderId { get; set; }
        public string Text { get; set; }
    }
}