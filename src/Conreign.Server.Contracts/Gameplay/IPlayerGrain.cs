using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Communication.Events;
using Orleans;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IPlayerGrain :
        IGrainWithGuidCompoundKey,
        IPlayer,
        IEventHandler<GameStarted>,
        IEventHandler<GameEnded>,
        IEventHandler<Connected>,
        IEventHandler<Disconnected>
    {
        Task Ping();
    }
}