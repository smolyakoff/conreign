using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IUser
    {
        Task<IPlayer> JoinRoom(string roomId);
    }
}