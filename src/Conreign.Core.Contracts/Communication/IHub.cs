using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Presence;

namespace Conreign.Core.Contracts.Communication
{
    public interface IHub : IConnectable
    {
        Task Notify(ISet<Guid> userIds, params IEvent[] events);
        Task NotifyEverybody(params IEvent[] events);
        Task NotifyEverybodyExcept(ISet<Guid> userIds, params IEvent[] events);
    }
}