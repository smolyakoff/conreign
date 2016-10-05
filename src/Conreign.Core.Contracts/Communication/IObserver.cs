using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IObserver
    {
        Task Notify(object @event);
    }
}