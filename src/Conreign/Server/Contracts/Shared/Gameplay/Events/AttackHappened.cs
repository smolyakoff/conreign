using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Gameplay.Data;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
[Immutable]
public class AttackHappened : IClientEvent, IRoomEvent
{
    public AttackHappened(string roomId, AttackOutcome outcome, string planetName, Guid attackerUserId,
        Guid? defenderUserId)
    {
        RoomId = roomId;
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
    public string RoomId { get; }
}