using Conreign.Server.Contracts.Shared.Communication;
using Conreign.Server.Contracts.Shared.Presence;

namespace Conreign.Server.Contracts.Server.Communication;

public interface IHub : IConnectable
{
    Task Notify(ISet<Guid> userIds, params IEvent[] events);
    Task NotifyEverybody(params IEvent[] events);
    Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events);
}