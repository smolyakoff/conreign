using System;

namespace Conreign.Core.Communication
{
    public static class TopicIds
    {
        public const string Global = "conreign/global";

        public static string Room(string roomId)
        {
            return $"conreign/rooms/{roomId}";
        }

        public static string Player(Guid userId, string roomId)
        {
            return $"{Room(roomId)}/{userId}";
        }
    }
}