using System;

namespace Conreign.Core.Client.Commands
{
    public class UpdatePlayerOptionsCommand
    {
        public Guid UserId { get; set; }
        public string Nickname { get; set; }
    }
}
