using System;

namespace Conreign.Core.Game
{
    public class UserState
    {
        public Guid UserId { get; set; }
        public string ConnectionId { get; set; }
        public string Nickname { get; set; }
    }
}