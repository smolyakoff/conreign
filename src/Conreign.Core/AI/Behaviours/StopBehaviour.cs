using System.Threading.Tasks;
using Conreign.Core.AI.Events;

namespace Conreign.Core.AI.Behaviours
{
    internal class StopBehaviour : IBotBehaviour<BotStopped>
    {
        public Task Handle(IBotNotification<BotStopped> notification)
        {
            notification.Context.Complete();
            return TaskCompleted.Completed;
        }
    }
}
