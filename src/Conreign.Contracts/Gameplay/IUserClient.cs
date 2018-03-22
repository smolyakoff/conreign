using System;
using System.Threading.Tasks;

namespace Conreign.Contracts.Gameplay
{
    public interface IUserClient
    {
        Task<IPlayerClient> JoinRoom(string roomId, Guid connectionId);
    }
}