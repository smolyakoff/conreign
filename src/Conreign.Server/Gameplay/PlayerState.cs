using System;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Gameplay
{
    public class PlayerState
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; }
        public RoomMode RoomMode { get; set; }
    }
}