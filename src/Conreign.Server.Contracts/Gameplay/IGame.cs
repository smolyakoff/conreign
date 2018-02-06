using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Data;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IGame : IRoom
    {
        Task Start(Guid userId, InitialGameData data);
        Task LaunchFleet(Guid userId, FleetData fleet);
        Task EndTurn(Guid userId, List<FleetData> fleets);
        Task CancelFleet(Guid userId, FleetCancelationData fleet);
        Task EndTurn(Guid userId);
    }
}