using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface IEventHandler
    {
    }

    public interface IEventHandler<in T> : IEventHandler where T : class 
    {
        Task Handle(T @event);
    }
}