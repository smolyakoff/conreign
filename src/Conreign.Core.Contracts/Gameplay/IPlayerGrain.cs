using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Conreign.Core.Contracts.Communication.Events;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;

namespace Conreign.Core.Contracts.Gameplay
{
    public interface IPlayerGrain :
        IGrainWithGuidCompoundKey,
        IPlayer,
        IEventHandler<GameStarted.Server>,
        IEventHandler<GameEnded>,
        IEventHandler<Connected>,
        IEventHandler<Disconnected>
    {
        Task Ping();
    }
}