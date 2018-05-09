using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Contracts.Communication;
using Conreign.Contracts.Presence;

namespace Conreign.Server.Contracts.Presence
{
    public interface IHub : IConnectable
    {
        Task Notify(ISet<Guid> userIds, params IEvent[] events);
        Task NotifyEverybody(params IEvent[] events);
        Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events);

        PresenceStatus GetPresenceStatus(Guid userId);
        IEnumerable<IClientEvent> GetEvents(Guid userId);
        Guid? LeaderUserId { get; }
        TimeSpan EveryoneOfflinePeriod { get; }
        void Reset(Dictionary<Guid, HubMemberState> members, List<Guid> joinOrder);
    }
}