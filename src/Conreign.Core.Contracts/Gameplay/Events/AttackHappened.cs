using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class AttackHappened : IClientEvent
    {
        public AttackHappened(AttackOutcome outcome, string planetName, Guid attackerUserId, Guid? defenderUserId)
        {
            Outcome = outcome;
            PlanetName = planetName;
            AttackerUserId = attackerUserId;
            DefenderUserId = defenderUserId;
        }

        public AttackOutcome Outcome { get; }
        public string PlanetName { get; }
        public Guid AttackerUserId { get; }
        public Guid? DefenderUserId { get; }
    }
}