using Conreign.Server.Contracts.Shared.Communication;

namespace Conreign.Server.Core.Presence;

public class EventState
{
    public HashSet<Guid> Recipients { get; set; }
    public IClientEvent Event { get; set; }
}