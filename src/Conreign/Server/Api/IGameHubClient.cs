using Conreign.Server.Contracts.Client;

namespace Conreign.Server.Api;

public interface IGameHubClient
{
    Task OnNext(MessageEnvelope messageEnvelope);
    Task OnError(Exception error);
    Task OnCompleted();
}