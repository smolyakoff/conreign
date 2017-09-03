using System;
using System.Threading.Tasks;

namespace Conreign.Contracts.Gameplay
{
    public interface IUser
    {
        Task<IPlayer> JoinRoom(string roomId, Guid connectionId);
    }
}