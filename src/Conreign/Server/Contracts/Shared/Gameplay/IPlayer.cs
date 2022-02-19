using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Contracts.Shared.Gameplay;

public interface IPlayer
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