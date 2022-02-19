namespace Conreign.Server.Contracts.Server.Communication;

public interface IEventHandler<in T> : IEventHandler where T : class
{
    Task Handle(T @event);
}