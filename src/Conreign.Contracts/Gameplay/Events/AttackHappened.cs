using System;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class AttackHappened : IClientEvent
    {
        public AttackHappened(AttackOutcome outcome, string planetName, Guid attackerUserId,
            Guid? defenderUserId)
        {
            Outcome = outcome;
            PlanetName = planetName;
            AttackerUserId = attackerUserId;
            DefenderUserId = defenderUserId;
            Timestamp = DateTime.UtcNow;
        }

        public AttackOutcome Outcome { get; }
        public string PlanetName { get; }
        public Guid AttackerUserId { get; }
        public Guid? DefenderUserId { get; }
        public DateTime Timestamp { get; }
    }
}