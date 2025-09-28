namespace Conreign.Server.Core.Presence;

public class HubState
{
    public string Id { get; set; }
    public Dictionary<Guid, HubMemberState> Members { get; set; } = new();
    public List<Guid> JoinOrder { get; set; } = new();
    public List<EventState> Events { get; set; } = new();
}