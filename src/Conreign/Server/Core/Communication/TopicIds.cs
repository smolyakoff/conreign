namespace Conreign.Server.Core.Communication;

public static class TopicIds
{
    public static string Room(string roomId)
    {
        return $"conreign/rooms/{roomId}";
    }

    public static string Player(Guid userId, string roomId)
    {
        return $"{Room(roomId)}/{userId}";
    }
}