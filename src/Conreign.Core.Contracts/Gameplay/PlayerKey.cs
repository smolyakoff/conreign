using System;

namespace Conreign.Core.Contracts.Gameplay
{
    public class PlayerKey
    {
        public PlayerKey(Guid userId, string roomId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id should not be empty", nameof(userId));
            }
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            UserId = userId;
            RoomId = roomId;
        }

        public Guid UserId { get; }

        public string RoomId { get; }
    }
}