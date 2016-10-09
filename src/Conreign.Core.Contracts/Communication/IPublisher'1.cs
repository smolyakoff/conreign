using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IPublisher<in T> where T : class
    {
        Task Notify(params T[] events);
    }
}