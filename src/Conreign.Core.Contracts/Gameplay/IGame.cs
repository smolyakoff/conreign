using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Data;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IGame : IRoom
    {
        Task LaunchFleet(Guid userId, FleetData fleet);
        Task EndTurn();
    }
}