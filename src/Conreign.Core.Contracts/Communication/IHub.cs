using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IHub
    {
        Task Notify(object @event, ISet<Guid> users);
        Task NotifyEverybody(object @event);
    }
}