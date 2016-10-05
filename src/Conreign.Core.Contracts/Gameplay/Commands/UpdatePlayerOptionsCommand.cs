using System;

namespace Conreign.Core.Contracts.Gameplay.Commands
{
    public class UpdatePlayerOptionsCommand
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
    }
}
