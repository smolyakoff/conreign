using System;

namespace Conreign.Core.Gameplay
{
    public class PlayerKey
    {
        public PlayerKey(string roomId, Guid userId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id cannot be empty.", nameof(userId));
            }
            RoomId = roomId;
            UserId = userId;
        }

        public string RoomId { get; }
        public Guid UserId { get;  }

        public override string ToString()
        {
            return $"{RoomId}.{UserId}";
        }
    }
}