using System;

namespace Conreign.Core.Contracts.Game.Messages
{
    public class PlayerState
    {
        public Guid UserId { get; set; }

        public bool IsOnline { get; set; }

        public string Nickname { get; set; }

        public string Color { get; set; }
    }
}