using System;

namespace Conreign.Core.Communication
{
    public static class SystemTopics
    {
        public const string Global = "global";

        public static string Player(Guid userId, string roomId)
        {
            if (userId == Guid.Empty)
            {
                throw new ArgumentException("User id should not be empty.", nameof(userId));
            }
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            return $"player:{roomId}:{userId}";
        }

        public static string Room(string roomId)
        {
            if (string.IsNullOrEmpty(roomId))
            {
                throw new ArgumentException("Room id cannot be null or empty.", nameof(roomId));
            }
            return $"room:{roomId}";
        }
    }
}
