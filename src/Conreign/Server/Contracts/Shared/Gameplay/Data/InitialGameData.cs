using Orleans.Concurrency;

namespace Conreign.Server.Contracts.Shared.Gameplay.Data;

[Serializable]
[Immutable]
public class InitialGameData
{
    public InitialGameData(
        Guid initiatorId,
        MapData map,
        List<PlayerData> players,
        Dictionary<Guid, HashSet<string>> hubMembers,
        List<Guid> hubJoinOrder)
    {
        InitiatorId = initiatorId;
        Map = map ?? throw new ArgumentNullException(nameof(map));
        Players = players ?? throw new ArgumentNullException(nameof(players));
        HubMembers = hubMembers ?? throw new ArgumentNullException(nameof(hubMembers));
        HubJoinOrder = hubJoinOrder ?? throw new ArgumentNullException(nameof(hubJoinOrder));
    }

    public Guid InitiatorId { get; }
    public MapData Map { get; }
    public List<PlayerData> Players { get; }
    public Dictionary<Guid, HashSet<string>> HubMembers { get; }
    public List<Guid> HubJoinOrder { get; }
}