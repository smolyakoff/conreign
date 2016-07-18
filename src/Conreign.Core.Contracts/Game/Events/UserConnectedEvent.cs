using System;

namespace Conreign.Core.Contracts.Game.Events
{
    public class UserConnectedEvent
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string RoomId { get; set; }
        public string ConnectionId { get; set; }
    }
}