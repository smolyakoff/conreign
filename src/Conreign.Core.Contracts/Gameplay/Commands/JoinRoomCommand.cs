using System;

namespace Conreign.Core.Contracts.Gameplay.Commands
{
    public class JoinRoomCommand
    {
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
        public string RoomId { get; set; }
    }
}