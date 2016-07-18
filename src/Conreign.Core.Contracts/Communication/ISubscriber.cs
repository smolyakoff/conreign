using MediatR;

namespace Conreign.Core.Contracts.Communication
{
    public interface ISubscriber<T> : IAsyncNotificationHandler<Event<T>>
    {
    }
}