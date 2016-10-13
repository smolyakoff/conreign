using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IBus : IPublisher<ISystemEvent>
    {
        Task Subscribe(IEventHandler handler);
        Task Unsubscribe(IEventHandler handler);
    }
}