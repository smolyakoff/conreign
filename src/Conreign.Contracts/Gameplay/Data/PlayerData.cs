using System;

namespace Conreign.Contracts.Gameplay.Data
{
    public class PlayerData
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string Color { get; set; }
        public PlayerType Type { get; set; }
    }
}