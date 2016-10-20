using System.Threading.Tasks;
using Conreign.Core.Contracts.Communication;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    public class LogBehaviour : IBotBehaviour<IClientEvent>
    {
        public Task Handle(IClientEvent @event, BotContext context)
        {
            context.Logger.Debug("[{ReadableId}-{UserId}]: Received {@Event}", context.ReadableId, context.UserId, @event);
            return Task.CompletedTask;
        }
    }
}
