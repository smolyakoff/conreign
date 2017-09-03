using System.Threading.Tasks;
using Conreign.Contracts.Communication;

namespace Conreign.LoadTest.Core.Behaviours
{
    public interface IBotBehaviour<in T> : IBotBehaviour where T : IClientEvent
    {
        Task Handle(IBotNotification<T> notification);
    }
}