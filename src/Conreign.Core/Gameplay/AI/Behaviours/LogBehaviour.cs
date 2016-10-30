using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;
using Orleans;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    public class LogBehaviour : IBotBehaviour<IClientEvent>
    {
        public Task Handle(IBotNotification<IClientEvent> notification)
        {
            var context = notification.Context;
            var @event = notification.Event;
            context.Logger.Debug("[{ReadableId}-{UserId}]: Received {@Event}",
                context.ReadableId,
                context.UserId == null ? "Anonymous" : context.UserId.ToString(),
                @event);
            return TaskCompleted.Completed;
        }
    }
}