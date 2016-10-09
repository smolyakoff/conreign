using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IEventHandler<in T> where T : class 
    {
        Task Handle(T @event);
    }
}