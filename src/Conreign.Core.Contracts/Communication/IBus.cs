using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IBus : ISystemPublisher
    {
        Task Subscribe<T>(IEventHandler<T> handler) where T : class, ISystemEvent;
        Task Unsubscribe<T>(IEventHandler<T> handler) where T : class, ISystemEvent;
        Task UnsubscribeAll(IEventHandler handler);
    }
}