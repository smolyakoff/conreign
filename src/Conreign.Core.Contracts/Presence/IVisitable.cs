using System;
using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Contracts.Presence
{
    public interface IVisitable
    {
        Task Join(Guid userId, IPublisher<IEvent> publisher);
        Task Leave(Guid userId);
    }
}
