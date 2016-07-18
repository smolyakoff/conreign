using System;
using System.Collections.Generic;

namespace Conreign.Core.Contracts.Game.Messages
{
    public class GetRoomStateResponse
    {
        public string RoomId { get; set; }

        public Guid OwnerId { get; set; }

        public List<PlanetState> Planets { get; set; }

        public List<PlayerState> Players { get; set; }

        public GameOptionsState GameOptions { get; set; }
    }
}