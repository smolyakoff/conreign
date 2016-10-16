using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Gameplay;

namespace Conreign.Core.Gameplay
{
    public class PlayerState
    {
        public HashSet<Guid> ConnectionIds { get; } = new HashSet<Guid>();
        public Guid UserId { get; set; }
        public string RoomId { get; set; }

        public ILobby Lobby { get; set; }
        public IGame Game { get; set; }

        public IRoom Room => (Game as IRoom) ?? Lobby;
    }
}