using System;

namespace Conreign.Core.Contracts.Game.Events
{
    public class JoinRoomRequestedEvent
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
    }
}