using System;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Core.Contracts.Gameplay.Events
{
    [Serializable]
    [Immutable]
    public class AttackHappened : IClientEvent, IRoomEvent
    {
        public AttackHappened(string roomId, AttackOutcome outcome, string planetName, Guid attackerUserId, Guid? defenderUserId)
        {
            RoomId = roomId;
            Outcome = outcome;
            PlanetName = planetName;
            AttackerUserId = attackerUserId;
            DefenderUserId = defenderUserId;
        }

        public AttackOutcome Outcome { get; }
        public string PlanetName { get; }
        public Guid AttackerUserId { get; }
        public Guid? DefenderUserId { get; }
        public string RoomId { get; }
    }
}