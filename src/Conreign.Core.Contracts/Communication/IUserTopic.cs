using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IUserTopic : ITopic
    {
        Task Broadcast(ISet<Guid> userIds, ISet<Guid> connectionIds, params IEvent[] events);
    }
}