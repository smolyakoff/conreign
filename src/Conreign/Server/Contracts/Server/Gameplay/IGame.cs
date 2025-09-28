using Conreign.Server.Contracts.Shared.Gameplay.Data;

namespace Conreign.Server.Contracts.Server.Gameplay;

public interface IGame : IRoom
{
    Task LaunchFleet(Guid userId, FleetData fleet);
    Task CancelFleet(Guid userId, FleetCancelationData fleet);
    Task EndTurn(Guid userId);
}