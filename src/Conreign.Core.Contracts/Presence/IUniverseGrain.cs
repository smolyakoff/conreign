using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Orleans;

namespace Conreign.Core.Contracts.Presence
{
    public interface IUniverseGrain : IGrainWithIntegerKey, IUniverse, IEventHandler<Connected>
    {
        Task Ping();
    }
}