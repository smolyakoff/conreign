using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface ISystemPublisherFactory
    {
        Task<IPublisher<IServerEvent>> CreateSystemPublisher(string topic);
    }
}