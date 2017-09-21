using System.Threading.Tasks;
using Conreign.Contracts.Gameplay.Events;

namespace Conreign.LoadTest.Core.Behaviours
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