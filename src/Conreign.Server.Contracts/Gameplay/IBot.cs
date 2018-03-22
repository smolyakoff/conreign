using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IBot : 
        IEventHandler<GameStarted>, 
        IEventHandler<TurnCalculationEnded>, 
        IEventHandler<PlayerDead>
    {
    }
}
