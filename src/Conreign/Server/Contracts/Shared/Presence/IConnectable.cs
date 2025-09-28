namespace Conreign.Server.Contracts.Shared.Presence;

public interface IConnectable
{
    Task Connect(Guid userId, string connectionId);
    Task Disconnect(Guid userId, string connectionId);
}