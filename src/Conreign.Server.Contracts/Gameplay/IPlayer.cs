using System;
using System.Threading.Tasks;
using Conreign.Contracts.Gameplay;
using Conreign.Contracts.Gameplay.Events;
using Conreign.Server.Contracts.Communication;

namespace Conreign.Server.Contracts.Gameplay
{
    public interface IPlayer : 
        IPlayerClient, 
        IEventHandler<GameStarted>,
        IEventHandler<GameEnded>
    {
        Task Connect(Guid connectionId);
        Task Disconnect(Guid connectionId);
    }
}
