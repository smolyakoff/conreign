namespace Conreign.Server.Contracts.Shared.Communication;

public interface IClientEvent : IEvent
{
    DateTime Timestamp { get; }
}