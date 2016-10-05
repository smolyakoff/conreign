using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Commands;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayer
    {
        Task JoinRoom(JoinRoomCommand command);
        Task UpdateOptions(UpdatePlayerOptionsCommand command);
        Task UpdateGameOptions();
        Task StartGame(StartGameCommand command);
        Task LaunchFleet();
        Task EndTurn();
        Task Write(WriteCommand command);
        Task<IRoomState> GetState();
    }
}