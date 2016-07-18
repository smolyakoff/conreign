using System.Threading.Tasks;

namespace Conreign.Core.Contracts.GameV2
{
    // Events
    public interface IRoom
    {
        Task Join();
        Task StartGame();
        Task UpdateGameParameters();
        Task RegenerateMap();
        Task Leave();
        Task AcceptPlayer();
        Task DeclinePlayer();
    }

    public interface IGame
    {
        Task GetState();
        Task MakeTurn();
    }

    public interface IChat
    {
        Task GetHistory();
        Task SendMessage();
        Task SendEvent();
    }
}
