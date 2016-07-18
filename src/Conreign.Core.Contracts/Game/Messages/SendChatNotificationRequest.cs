using System;
using System.Collections.Generic;

namespace Conreign.Core.Contracts.Game.Messages
{
    public class SendChatNotificationRequest
    {
        public string ChatId { get; set; }

        public object Data { get; set; }

        public HashSet<Guid> UserIds { get; set; }
    }
}