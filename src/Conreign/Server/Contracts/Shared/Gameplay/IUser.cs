namespace Conreign.Server.Contracts.Shared.Gameplay;

public interface IUser
{
    Task<IPlayer> JoinRoom(string roomId, string connectionId);
}