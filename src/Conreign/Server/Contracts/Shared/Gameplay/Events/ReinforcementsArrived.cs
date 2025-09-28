using Conreign.Server.Contracts.Shared.Communication;
using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Events;

[Serializable]
[Immutable]
[Private]
public class ReinforcementsArrived : IClientEvent, IRoomEvent
{
    public ReinforcementsArrived(string roomId, string planetName, Guid ownerId, int ships)
    {
        RoomId = roomId;
        PlanetName = planetName;
        OwnerId = ownerId;
        Ships = ships;
        Timestamp = DateTime.UtcNow;
    }

    public string PlanetName { get; }
    public Guid OwnerId { get; }
    public int Ships { get; }
    public DateTime Timestamp { get; }
    public string RoomId { get; }
}