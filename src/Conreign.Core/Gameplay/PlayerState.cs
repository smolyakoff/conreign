using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Gameplay
{
    public class PlayerState
    {
        public HashSet<string> ConnectionIds { get; } = new HashSet<string>();
        public Guid UserId { get; set; }
        public string RoomId { get; set; }
        public IRoom Room { get; set; }
    }
}