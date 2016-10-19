using System.Threading.Tasks;

namespace Conreign.Core.Contracts.Communication
{
    public interface ITopic
    {
        Task Send(params IServerEvent[] events);
    }
}