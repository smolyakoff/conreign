using Conreign.Server.Contracts.Shared.Communication;

namespace Conreign.Server.Contracts.Server.Communication;

public interface ITopic
{
    Task Send(params IServerEvent[] events);
}