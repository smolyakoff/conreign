using System;

namespace Conreign.Core.Contracts.Gameplay.Commands
{
    public class WriteCommand
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; }
        public string Text { get; set; }
    }
}