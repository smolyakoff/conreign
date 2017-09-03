using System.Threading.Tasks;
using Conreign.Contracts.Communication;

namespace Conreign.Server.Contracts.Communication
{
    public interface ITopic
    {
        Task Send(params IServerEvent[] events);
    }
}