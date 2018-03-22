using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Contracts.Gameplay
{
    public interface IPlayerClient
    {
        Task UpdateOptions(PlayerOptionsData options);
        Task UpdateGameOptions(GameOptionsData options);
        Task StartGame();
        Task LaunchFleet(FleetData fleet);
        Task CancelFleet(FleetCancelationData fleetCancelation);
        Task EndTurn();
        Task SendMessage(TextMessageData textMessage);
        Task<IRoomData> GetState();
    }
}