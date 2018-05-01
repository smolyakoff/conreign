using System;
using System.Collections.Generic;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    public class PlayerListChanged : IClientEvent, IRoomEvent
    {
        public PlayerListChanged(string roomId, List<PlayerData> playersJoined, List<Guid> playersLeft)
        {
            RoomId = roomId ?? throw new ArgumentNullException(nameof(roomId));
            PlayersJoined = playersJoined ?? throw new ArgumentNullException(nameof(playersJoined));
            PlayersLeft = playersLeft ?? throw new ArgumentNullException(nameof(playersLeft));
        }

        public DateTime Timestamp { get; } = DateTime.UtcNow;
        public string RoomId { get; }
        public List<PlayerData> PlayersJoined { get; }
        public List<Guid> PlayersLeft { get; }
    }
}
