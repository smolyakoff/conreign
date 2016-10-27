using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    public class StopOnGameEndBehaviour : IBotBehaviour<GameEnded>
    {
        public Task Handle(IBotNotification<GameEnded> notification)
        {
            notification.Context.Complete();
            return Task.CompletedTask;
        }
    }
}
