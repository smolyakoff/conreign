using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IObserver<in T>
    {
        Task Notify(params T[] events);
    }
}