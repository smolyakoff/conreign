using System;
using System.Collections.Generic;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Gameplay
{
    public interface IGameState
    {
        MapData Map { get; set; }
        List<PlayerData> Players { get; set; }
        Dictionary<Guid, PlayerGameState> PlayerStates { get; set; }
        string RoomId { get; set; }
        GameStatus Status { get; set; }
        int Turn { get; set; }
    }
}