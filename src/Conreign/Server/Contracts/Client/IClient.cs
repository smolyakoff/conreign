namespace Conreign.Server.Contracts.Client;

public interface IClient
{
    Task<IClientConnection> Connect(Guid connectionId);
}