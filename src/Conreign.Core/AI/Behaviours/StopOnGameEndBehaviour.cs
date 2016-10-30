using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Events;
using Orleans;

namespace Conreign.Core.AI.Behaviours
{
    public class StopOnGameEndBehaviour : IBotBehaviour<GameEnded>
    {
        public Task Handle(IBotNotification<GameEnded> notification)
        {
            notification.Context.Complete();
            return TaskCompleted.Completed;
        }
    }
}