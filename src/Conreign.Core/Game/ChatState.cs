using System;
using System.Collections.Generic;

namespace Conreign.Core.Game
{
    public class ChatState
    {
        public ChatState(string chatId)
        {
            ChatId = chatId;
            Users = new Dictionary<Guid, UserState>();
        }

        public string ChatId { get; }

        public Dictionary<Guid, UserState> Users { get; }
    }
}