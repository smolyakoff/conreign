
using System.Threading.Tasks;
using Conreign.Core.Contracts.Gameplay.Events;

namespace Conreign.Core.Gameplay.AI.Behaviours
{
    public class StopOnGameEndBehaviour : IBotBehaviour<GameEnded>
    {
        public Task Handle(GameEnded @event, BotContext context)
        {
            context.Complete();
            return Task.CompletedTask;
        }
    }
}
