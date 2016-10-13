using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IHub
    {
        Task Notify(ISet<Guid> users, params IEvent[] events);
        Task NotifyEverybody(params IEvent[] events);
        Task NotifyEverybodyExcept(ISet<Guid> users, params IEvent[] events);
    }
}