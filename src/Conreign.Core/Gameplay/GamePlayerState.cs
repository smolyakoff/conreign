using System;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Gameplay
{
    public class GamePlayerState
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
        public string Color { get; set; }
    }
}