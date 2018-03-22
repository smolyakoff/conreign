using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;
using Conreign.Server.Contracts.Communication.Events;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IPlayer : 
        IPlayerClient, 
        IEventHandler<GameStarted>,
        IEventHandler<GameEnded>,
        IEventHandler<Connected>,
        IEventHandler<Disconnected>
    {
    }
}
