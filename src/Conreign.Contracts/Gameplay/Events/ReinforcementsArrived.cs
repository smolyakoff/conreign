using System;
using Conreign.Contracts.Communication;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    [Private]
    public class ReinforcementsArrived : IClientEvent
    {
        public ReinforcementsArrived(string planetName, Guid ownerId, int ships)
        {
            PlanetName = planetName;
            OwnerId = ownerId;
            Ships = ships;
            Timestamp = DateTime.UtcNow;
        }

        public string PlanetName { get; }
        public Guid OwnerId { get; }
        public int Ships { get; }
        public DateTime Timestamp { get; }
    }
}