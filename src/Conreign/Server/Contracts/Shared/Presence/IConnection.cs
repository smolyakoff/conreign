namespace Conreign.Server.Contracts.Shared.Presence;

public interface IConnection
{
    Task Connect(string topicId);
    Task Disconnect();
}