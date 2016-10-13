using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface ISystemPublisherFactory
    {
        Task<IPublisher<ISystemEvent>> CreateSystemPublisher(string topic);
    }
}