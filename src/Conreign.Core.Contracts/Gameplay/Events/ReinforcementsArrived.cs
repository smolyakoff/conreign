using System;
using Conreign.Core.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class ReinforcementsArrived : IClientEvent
    {
        public ReinforcementsArrived(string planetName, Guid ownerId, int ships)
        {
            PlanetName = planetName;
            OwnerId = ownerId;
            Ships = ships;
        }

        public string PlanetName { get; }
        public Guid OwnerId { get; }
        public int Ships { get; }
    }
}