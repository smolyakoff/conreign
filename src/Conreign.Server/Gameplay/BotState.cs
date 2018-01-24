using System;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Gameplay
{
    public class BotState
    {
        public Guid UserId { get; set; }
        public string RoomId { get; set; }
        public bool IsDead { get; set; }
        public MapData Map { get; set; } = new MapData();
    }
}