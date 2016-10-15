using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IBus : IPublisher<IServerEvent>
    {
        Task Subscribe(IEventHandler handler);
        Task Unsubscribe(IEventHandler handler);
    }
}