namespace Conreign.Server.Core.Presence;

public class HubMemberState
{
    public HashSet<string> ConnectionIds { get; set; } = new();
    public DateTime ConnectionsChangedAt { get; set; }
}