using System;

namespace Conreign.Core.Contracts.Gameplay.Data
{
    public class PlayerOptionsState
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string Color { get; set; }
    }
}