using Conreign.Server.Contracts.Shared.Communication;

namespace Conreign.Server.Contracts.Server.Communication;

public interface IUserTopic : ITopic
{
    Task Broadcast(ISet<Guid> userIds, ISet<string> connectionIds, params IEvent[] events);
}