using System;
using System.Collections.Generic;
using Conreign.Core.Contracts.Game.Messages;

namespace Conreign.Core.Game
{
    public class RoomState
    {
        public RoomState(string roomId)
        {
            RoomId = roomId;
            var playerComparer = new KeyEqualityComparer<PlayerState, Guid>(x => x.UserId);
            Players = new HashSet<PlayerState>(playerComparer);
            Candidates = new HashSet<PlayerState>(playerComparer);
            Planets = new List<PlanetState>();
        }

        public string RoomId { get; }

        public bool IsGameStarted { get; set; }

        public HashSet<PlayerState> Players { get; }

        public PlayerState Owner { get; set; }

        public HashSet<PlayerState> Candidates { get; }

        public List<PlanetState> Planets { get; }

        public GameOptionsState GameOptions { get; set; }
    }
}