using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IGame : IRoom
    {
        Task LaunchFleet(Guid userId, FleetData fleet);
        Task CancelFleet(Guid userId, FleetCancelationData fleet);
        Task EndTurn(Guid userId);
    }
}